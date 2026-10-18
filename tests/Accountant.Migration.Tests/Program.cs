using Accountant;
using Newtonsoft.Json.Linq;

var assertions = 0;
void Assert(bool condition, string message)
{
    assertions++;
    if (!condition) throw new Exception(message);
}
var root = Path.Combine(Path.GetTempPath(), "AccountantCN-migration-tests-" + Guid.NewGuid().ToString("N"));
Directory.CreateDirectory(root);
try
{
    string Fixture(string name, bool malformed = false)
    {
        var parent = Path.Combine(root, name);
        Directory.CreateDirectory(Path.Combine(parent, "Accountant", "crops_plot"));
        File.WriteAllText(Path.Combine(parent, "Accountant.json"), malformed ? "{" : """
            {
              "$type": "Accountant.AccountantConfiguration, Accountant",
              "Note": "Accountant.AccountantConfiguration, Accountant",
              "Colors": {"$type": "System.Collections.Generic.Dictionary`2[[Accountant.Gui.ColorId, Accountant],[System.UInt32, System.Private.CoreLib]], System.Private.CoreLib"},
              "Data": {"$type": "Accountant.Structs.CropData, Accountant.GameData"}
            }
            """);
        File.WriteAllText(Path.Combine(parent, "Accountant", "crops_plot", "fixture.json"),
            """{"PlantTime":"2026-10-18T18:47:00Z","PlantIds":[8165,8165,8165,4788,4788,4788,4788,4788]}""");
        return parent;
    }

    foreach (var emptyDirectory in new[] { false, true })
    {
        var parent = Fixture(emptyDirectory ? "empty" : "fresh");
        var target = Path.Combine(parent, "AccountantCN");
        if (emptyDirectory) Directory.CreateDirectory(target);
        var original = File.ReadAllBytes(Path.Combine(parent, "Accountant.json"));
        Assert(LegacyConfigMigration.Import(emptyDirectory ? target + Path.DirectorySeparatorChar : target), "Existing legacy configuration must be imported");
        var config = JObject.Parse(File.ReadAllText(Path.Combine(parent, "AccountantCN.json")));
        Assert((string?)config["$type"] == "Accountant.AccountantConfiguration, AccountantCN", "Rebind main assembly");
        Assert(((string?)config["Colors"]?["$type"])!.Contains("Accountant.Gui.ColorId, AccountantCN]"), "Rebind nested generic types");
        Assert((string?)config["Data"]?["$type"] == "Accountant.Structs.CropData, Accountant.GameData", "Keep other assemblies");
        Assert((string?)config["Note"] == "Accountant.AccountantConfiguration, Accountant", "Keep ordinary user text");
        Assert(File.ReadAllBytes(Path.Combine(parent, "Accountant.json")).SequenceEqual(original), "Preserve original settings");
        var crops = JObject.Parse(File.ReadAllText(Path.Combine(target, "crops_plot", "fixture.json")));
        Assert(crops["PlantIds"]!.Count() == 8, "Preserve all eight crops");
        Assert(File.ReadAllText(Path.Combine(target, "crops_plot", "fixture.json")).Contains("2026-10-18T18:47:00Z"), "Preserve timestamp text");
        Assert(!LegacyConfigMigration.Import(target), "Do not reimport over an existing CN configuration");
    }

    var existing = Fixture("existing-cn");
    File.WriteAllText(Path.Combine(existing, "AccountantCN.json"), "{\"OwnSetting\":42}");
    Assert(!LegacyConfigMigration.Import(Path.Combine(existing, "AccountantCN")), "Keep existing CN settings");
    Assert(File.ReadAllText(Path.Combine(existing, "AccountantCN.json")) == "{\"OwnSetting\":42}", "CN settings must be unchanged");

    var timerOnly = Fixture("existing-cn-timers");
    Directory.CreateDirectory(Path.Combine(timerOnly, "AccountantCN"));
    File.WriteAllText(Path.Combine(timerOnly, "AccountantCN", "own.json"), "{}");
    Assert(!LegacyConfigMigration.Import(Path.Combine(timerOnly, "AccountantCN")), "Keep an existing CN data directory");

    var broken = Fixture("malformed", true);
    var failed = false;
    try { LegacyConfigMigration.Import(Path.Combine(broken, "AccountantCN")); }
    catch (Newtonsoft.Json.JsonException) { failed = true; }
    Assert(failed && !File.Exists(Path.Combine(broken, "AccountantCN.json")), "Malformed source must not publish a partial config");
    Assert(File.ReadAllText(Path.Combine(broken, "Accountant.json")) == "{", "Malformed original must remain untouched");
    Assert(!LegacyConfigMigration.Import(Path.Combine(root, "no-legacy", "AccountantCN")), "Fresh installs need no migration");
    Console.WriteLine($"Passed {assertions} migration assertions.");
}
finally
{
    if (!Path.GetFullPath(root).StartsWith(Path.GetFullPath(Path.GetTempPath()), StringComparison.OrdinalIgnoreCase))
        throw new InvalidOperationException("Unexpected test cleanup path");
    Directory.Delete(root, true);
}
