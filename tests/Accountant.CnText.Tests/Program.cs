using Accountant.Internal;

var assertions = 0;
void Assert(bool condition, string message)
{
    assertions++;
    if (!condition) throw new Exception(message);
}

foreach (var seed in new[] { "克拉卡萝卜种子", "巫茄种子", "坦塔尔矮瓜种子", "旧世界无花果种子", "萨维奈圆葱种子", "皇家可可豆种子" })
{
    var match = CnCropText.Seed.Match($"确定要将{seed}种植在3级萨纳兰土壤中吗？");
    Assert(match.Success && match.Groups["seeds"].Value == seed && match.Groups["soil"].Value == "3级萨纳兰土壤", seed);
}
foreach (var patch in Enumerable.Range(1, 3))
foreach (var bed in Enumerable.Range(1, 8))
{
    var parsed = CnCropText.ParsePatch($"园圃{patch}：地垄{bed}");
    Assert(parsed.SequenceEqual(new[] { patch.ToString(), bed.ToString() }), $"{patch}/{bed}");
}
foreach (var invalid in new[] { "园圃0：地垄1", "园圃4：地垄1", "园圃1：地垄0", "园圃1：地垄9", "园圃1：地垄10", "", "无关对话" })
    Assert(CnCropText.ParsePatch(invalid).Count == 0, invalid);
foreach (var name in new[] { "巫茄", "克拉卡萝卜", "坦塔尔矮瓜", "旧世界无花果", "萨维奈圆葱", "皇家可可豆" })
foreach (var suffix in new[] { CnCropText.HealthySuffix, CnCropText.UnhealthySuffix })
    Assert(CnCropText.ParsePlant(name + suffix).SequenceEqual(new[] { name }), name + suffix);
Assert(CnCropText.ParsePlant("无关对话").Count == 0, "Unrelated status");
Assert(!CnCropText.Seed.IsMatch("确定要处理掉作物吗？"), "Removal is not planting");
var wheel = CnDialogText.Wheel.Match("确定要将“1级战斗转轮”设置到转轮台上吗？");
Assert(wheel.Success && wheel.Groups["wheel"].Value == "1级战斗转轮", "CN wheel confirmation");
foreach (var ticket in new[] { "0000", "0007", "1234", "9999" })
{
    var match = CnDialogText.Jumbo.Match($"确定要购买{ticket}号仙人仙彩吗？");
    Assert(match.Success && match.Groups["ticket"].Value == ticket, "CN Jumbo ticket " + ticket);
}
foreach (var invalid in new[] { "确定要购买123号仙人仙彩吗？", "确定要购买12345号仙人仙彩吗？", "无关对话" })
    Assert(!CnDialogText.Jumbo.IsMatch(invalid), "Invalid Jumbo ticket");
Console.WriteLine($"Passed {assertions} CN text assertions.");
