using System.Collections.Generic;
using System.Text.RegularExpressions;
using Dalamud.Game;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using Lumina.Excel.Sheets;

namespace Accountant.Data;

public class Wheels
{
    private const int NumWheels = 45;

    private readonly Dictionary<uint, (Item Item, string Name, byte Grade)>   _idToItem   = new(NumWheels);
    private readonly Dictionary<string, (Item Item, string Name, byte Grade)> _nameToItem = new(NumWheels);

    internal (Item, string Name, byte Grade) Find(uint itemId)
        => _idToItem.TryGetValue(itemId, out var wheel) ? wheel : (new Item(), string.Empty, (byte)0);

    internal (Item, string Name, byte Grade) Find(string name)
        => _nameToItem.TryGetValue(name.ToLowerInvariant(), out var wheel) ? wheel : (new Item(), string.Empty, (byte)0);

    private static readonly Regex WheelRegex = new(@"^grade (?<grade>\d) wheel of", RegexOptions.Compiled);
    private static readonly Regex PrimedWheelRegex = new(@"^primed grade (?<grade>\d) wheel of", RegexOptions.Compiled);

    internal Wheels(IDataManager gameData)
    {
        if (gameData.Language == ClientLanguage.ChineseSimplified)
        {
            InitializeChinese(gameData);
            return;
        }

        var items        = gameData.GetExcelSheet<Item>(ClientLanguage.English);
        var itemsLang    = gameData.GetExcelSheet<Item>();
        var primedWheels = new List<(string, uint)>(50);
        var englishDict  = new Dictionary<string, (Item Item, string Name, byte Grade)>(50);
        foreach (var item in items)
        {
            var englishName  = item.Name.ExtractText().ToLowerInvariant();
            var match = WheelRegex.Match(englishName);
            if (!match.Success)
            {
                match = PrimedWheelRegex.Match(englishName);
                if (match.Success)
                    primedWheels.Add((englishName.Replace("primed ", ""), item.RowId));
                continue;
            }

            var grade    = (byte)(match.Groups["grade"].Value[0] - '0');
            var itemLang = itemsLang.GetRow(item.RowId);
            var name     = itemLang.Name.ToDalamudString().TextValue;
            var singular = itemLang.Name.ToDalamudString().TextValue.ToLowerInvariant();
            _idToItem.TryAdd(itemLang.RowId, (itemLang, name, grade));
            _nameToItem.TryAdd(name.ToLowerInvariant(), (itemLang, name, grade));
            _nameToItem.TryAdd(singular,                (itemLang, name, grade));
            englishDict.TryAdd(englishName,             (itemLang, name, grade));
        }

        foreach (var (englishName, primedWheel) in primedWheels)
        {
            var itemLang = itemsLang.GetRow(primedWheel);
            var fullName = itemLang.Name.ToDalamudString().TextValue.ToLowerInvariant();
            var singular = itemLang.Name.ToDalamudString().TextValue.ToLowerInvariant();
            if (!englishDict.TryGetValue(englishName, out var data))
                continue;
            _nameToItem.TryAdd(fullName, data);
            _nameToItem.TryAdd(singular, data);
        }
    }

    private static readonly Regex ChineseWheelRegex = new(@"^(?<grade>[1-3])级.+以太转轮$", RegexOptions.Compiled);

    private void InitializeChinese(IDataManager gameData)
    {
        // CN clients need not have the English Item sheet or a matching en-lite pack.
        var items = gameData.Excel.GetSheet<Item>(gameData.Language.ToLumina());
        foreach (var item in items)
        {
            var name = item.Name.ExtractText();
            var match = ChineseWheelRegex.Match(name);
            if (!match.Success)
                continue;

            var grade = (byte)(match.Groups["grade"].Value[0] - '0');
            var entry = (item, name, grade);
            _idToItem.TryAdd(item.RowId, entry);
            _nameToItem.TryAdd(name, entry);
        }

        foreach (var item in items)
        {
            var name = item.Name.ExtractText();
            if (name.Contains("充能转轮") && _nameToItem.TryGetValue(name.Replace("充能转轮", "以太转轮"), out var entry))
                _nameToItem.TryAdd(name, entry);
        }
    }
}
