using System.Text.RegularExpressions;

namespace Accountant.Internal;

// Restored from decorwdyun/Accountant's CN localization, submitted in D17 PR #335.
internal static class CnDialogText
{
    internal static readonly Regex Wheel = new(
        @"确定要将“(?<wheel>.+?)”设置到转轮台上吗？",
        RegexOptions.Compiled | RegexOptions.ExplicitCapture);

    internal static readonly Regex Jumbo = new(
        @"(?<!\d)(?<ticket>\d{4})号仙人仙彩吗？",
        RegexOptions.Compiled | RegexOptions.ExplicitCapture);
}
