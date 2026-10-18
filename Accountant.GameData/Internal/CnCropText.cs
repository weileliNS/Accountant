using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Accountant.Internal;

// Templates verified against CN client Addon rows 6413/6420 and gardening rows 8/9.
internal static class CnCropText
{
    internal const string HealthySuffix = "正茁壮成长。";
    internal const string UnhealthySuffix = "的状态不太好……";

    internal static readonly Regex Seed = new(
        @"^确定要将(?<seeds>.+?)种植在(?<soil>.+?)中吗？$",
        RegexOptions.Compiled | RegexOptions.ExplicitCapture);

    private static readonly Regex Patch = new(
        @"^园圃(?<patch>[1-3])：地垄(?<bed>[1-8])$",
        RegexOptions.Compiled | RegexOptions.ExplicitCapture);

    private static readonly Regex Plant = new(
        @"^(?<plant>.+?)(?:正茁壮成长。|的状态不太好……)$",
        RegexOptions.Compiled | RegexOptions.ExplicitCapture);

    internal static IList<string> ParsePatch(string text)
    {
        var match = Patch.Match(text);
        return match.Success
            ? new[] { match.Groups["patch"].Value, match.Groups["bed"].Value }
            : Array.Empty<string>();
    }

    internal static IList<string> ParsePlant(string text)
    {
        var match = Plant.Match(text);
        return match.Success ? new[] { match.Groups["plant"].Value } : Array.Empty<string>();
    }
}
