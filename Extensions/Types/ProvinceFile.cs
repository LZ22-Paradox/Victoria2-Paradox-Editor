using System;
using System.Collections.Generic;
using System.IO;

namespace Paradox_Editor.Extensions.Types;

public class ProvinceFile
{
    // FILE I/O
    public int ProvinceID { get; set; } //✓
    public string ProvinceName { get; set; }
    public string HistoryFilePath { get; set; } //✓
    public string ProvinceFileName { get; set; } //✓

    // IO Needed for CSV Processing (Especially do not touch)
    public uint Color { get; set; }

    // HISTORY DATA
    public string Owner { get; set; }
    public string Controller { get; set; }
    public List<Core> Cores = new();
    public string TradeGoods { get; set; }
    public int LifeRating { get; set; }
    public string Terrain { get; set; }
    public int Colonial { get; set; }
    public readonly List<StateBuilding> State_Buildings = new();
    public int Naval_Base { get; set; }
    public int Fort { get; set; }
    public int Railroad { get; set; }

    //Population info
    public void AppendFromCSV(ProvinceCSVDefinition record)
    {
        if (string.IsNullOrWhiteSpace(record.province) ||
            !uint.TryParse(record.Red.Replace(".", ""), out var red) ||
            !uint.TryParse(record.Green.Replace(".", ""), out var green) ||
            !uint.TryParse(record.Blue.Replace(".", ""), out var blue))
            return;

        // Faulty. Some provinces bug. (Tested Vanilla Vic2)
        Color = (0xFFu << 24) | ((red & 0xFF) << 16) | ((green & 0xFF) << 8) | (blue & 0xFF);
        ProvinceName = record.name;
        // Debug.WriteLine("(RGB: {0}, ID: {1}, NAME: {2})", color, record.province, record.name);
    }

    public ProvinceFile PopulateHistoryData()
    {
        List<Core> tempCoresList = new();
        using StreamReader sr = new(HistoryFilePath);
        while (sr.ReadLine() is { } line)
        {
            string value;
            if (line.Replace("\t", "").Contains("}") || string.IsNullOrWhiteSpace(line))
                value = null;
            else
            {
                int index = line.IndexOf("#", StringComparison.Ordinal);
                if (index >= 0)
                    line = line[..index];

                var seperatedLines = line.Replace(" ", "").Split('=');
                if (seperatedLines.Length < 2)
                    break;

                value = seperatedLines[1];
            }

            // Sofar there are two trees:
            // - History
            // ...
            //   - Building
            //     ...
            // ...
            if (isReadingBuildings == false)
                ReadHistoryLines(line, value, tempCoresList);
            else
                ReadBuildingLines(line, value);
        }

        Cores = tempCoresList;
        return this;
    }

    private bool isReadingBuildings = false;
    private StateBuilding tempStateBuilding;

    private void ReadHistoryLines(string line, string value, List<Core> tempCoresList)
    {
        switch (line)
        {
            // WARN: It is ignoring state-buildings!
            case not null when line.StartsWith("state_building"):
                isReadingBuildings = true;
                tempStateBuilding = new StateBuilding();
                break;
            case not null when line.StartsWith("owner"):
                Owner = value.RemoveWhitespace();
                break;
            case not null when line.StartsWith("controller"):
                Controller = value.RemoveWhitespace();
                break;
            case not null when line.StartsWith("trade_goods"):
                TradeGoods = value;
                break;
            case not null when line.StartsWith("life_rating"):
                LifeRating = (short)Convert.ToDouble(value);
                break;
            case not null when line.StartsWith("colonial"):
                if (int.TryParse(value, out int result))
                    Colonial = Convert.ToInt16(result);
                else if (value != null && value.Equals("yes"))
                    Colonial = Convert.ToInt16(1);
                else
                    Colonial = Convert.ToInt16(0);
                break;
            case not null when line.StartsWith("terrain"):
                Terrain = value;
                break;
            case not null when line.StartsWith("add_core"):
                tempCoresList.Add(new Core(value));
                break;
            case not null when line.StartsWith("naval_base"):
                Naval_Base = Convert.ToInt16(value);
                break;
            case not null when line.StartsWith("fort"):
                Fort = Convert.ToInt16(value);
                break;
            case not null when line.StartsWith("railroad"):
                Railroad = Convert.ToInt16(value);
                break;
        }
    }

    private void ReadBuildingLines(string line, string value)
    {
        switch (line)
        {
            case not null when line.Contains("level"):
                tempStateBuilding.Level = value;
                break;
            case not null when line.Contains("building"):
                tempStateBuilding.Building = value;
                break;
            case not null when line.Contains("upgrade"):
                tempStateBuilding.Upgrade = value;
                break;
            default: // Stop reading buildings.
                State_Buildings.Add(tempStateBuilding);
                break;
        }
    }
}