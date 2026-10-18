using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Accountant;

internal static class LegacyConfigMigration
{
    private static readonly Regex LegacyAssembly = new(@"(,\s*)Accountant(?=\s*(?:,|\]|$))");

    internal static bool Import(string configDirectory)
    {
        var target = Path.TrimEndingDirectorySeparator(Path.GetFullPath(configDirectory));
        if (Path.GetFileName(target) != "AccountantCN")
            throw new ArgumentException("Unexpected CN configuration directory.", nameof(configDirectory));

        var parent = Path.GetDirectoryName(target)!;
        var targetConfig = Path.Combine(parent, "AccountantCN.json");
        if (File.Exists(targetConfig) || Directory.Exists(target) && Directory.EnumerateFileSystemEntries(target).Any())
            return false;

        var source = Path.Combine(parent, "Accountant");
        var sourceConfig = Path.Combine(parent, "Accountant.json");
        if (!Directory.Exists(source) && !File.Exists(sourceConfig))
            return false;

        // Prepare every file first. Failed imports leave the originals untouched.
        var staging = target + ".import-" + Guid.NewGuid().ToString("N");
        Directory.CreateDirectory(staging);
        if (Directory.Exists(source))
        {
            foreach (var file in Directory.EnumerateFiles(source, "*", SearchOption.AllDirectories))
            {
                var destination = Path.Combine(staging, Path.GetRelativePath(source, file));
                Directory.CreateDirectory(Path.GetDirectoryName(destination)!);
                CopyFile(file, destination);
            }
        }
        if (File.Exists(sourceConfig))
            CopyFile(sourceConfig, staging + ".json");

        if (Directory.Exists(target))
            Directory.Delete(target); // The existing directory was checked to be empty.
        Directory.Move(staging, target);
        if (File.Exists(staging + ".json"))
            File.Move(staging + ".json", targetConfig);
        return true;
    }

    private static void CopyFile(string source, string destination)
    {
        if (!Path.GetExtension(source).Equals(".json", StringComparison.OrdinalIgnoreCase))
        {
            File.Copy(source, destination);
            return;
        }

        using var reader = new JsonTextReader(File.OpenText(source)) { DateParseHandling = DateParseHandling.None };
        var token = JToken.ReadFrom(reader);
        RewriteTypes(token);
        File.WriteAllText(destination, token.ToString(Formatting.Indented), new UTF8Encoding(false));
    }

    private static void RewriteTypes(JToken token)
    {
        if (token is JObject obj)
        {
            foreach (var property in obj.Properties())
            {
                if (property.Name == "$type" && property.Value.Type == JTokenType.String)
                    property.Value = LegacyAssembly.Replace(property.Value.Value<string>()!, "${1}AccountantCN");
                else
                    RewriteTypes(property.Value);
            }
        }
        else if (token is JArray array)
        {
            foreach (var child in array)
                RewriteTypes(child);
        }
    }
}
