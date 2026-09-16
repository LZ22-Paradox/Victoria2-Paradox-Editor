using System;
using System.Collections.Generic;
using System.IO;
using Paradox_Editor.Extensions;
using Paradox_Editor.Types;

namespace Paradox_Editor.Parsers;

public class HistoryFiller : IDisposable
{
    public void PopulateHistoryData(uint id, string historyFilePath, ProvinceDatabase database)
    {
        List<Tag> tempCoresList = [];
        using StreamReader sr = new(historyFilePath);
        while (sr.ReadLine() is { } line)
        {
            string value;
            if (line.Replace("\t", "").Contains('}') || string.IsNullOrWhiteSpace(line))
                value = "";
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
                FillHistoryFromLine(id, line, value, tempCoresList, database);
            else
                FillBuildingFromLine(id, line, value, database);
        }

        database.Cores[id] = tempCoresList;
    }

    private bool isReadingBuildings = false;
    private StateBuilding tempStateBuilding = new();

    private void FillHistoryFromLine(uint id, string line, string value, List<Tag> coresTemp, ProvinceDatabase database)
    {
        if (string.IsNullOrEmpty(line))
            return;

        switch (line)
        {
            // WARN: It is ignoring state-buildings!
            case not null when line.StartsWith("state_building"):
                isReadingBuildings = true;
                tempStateBuilding = new StateBuilding();
                break;
            case not null when line.StartsWith("owner"):
                database.SetOwner(id, value.RemoveWhitespace());
                break;
            case not null when line.StartsWith("controller"):
                database.SetController(id, value.RemoveWhitespace());
                break;
            case not null when line.StartsWith("trade_goods"):
                database.SetTradeGood(id, value);
                break;
            case not null when line.StartsWith("life_rating"):
                database.SetLifeRating(id, (short)Convert.ToDouble(value));
                break;
            case not null when line.StartsWith("colonial"):
                if (short.TryParse(value, out short result)) result = Convert.ToInt16(result);
                else if (!string.IsNullOrEmpty(value) && value.Equals("yes")) result = Convert.ToInt16(1);
                else result = Convert.ToInt16(0);
                database.SetColonial(id, result);
                break;
            case not null when line.StartsWith("terrain"):
                database.SetTerrain(id, value);
                break;
            case not null when line.StartsWith("add_core"):
                coresTemp.Add(new Tag(value));
                break;
            case not null when line.StartsWith("naval_base"):
                database.SetNavalBaseLevel(id, Convert.ToInt16(value));
                break;
            case not null when line.StartsWith("fort"):
                database.SetFortLevel(id, Convert.ToInt16(value));
                break;
            case not null when line.StartsWith("railroad"):
                database.SetRailroadLevel(id, Convert.ToInt16(value));
                break;
        }
    }

    private void FillBuildingFromLine(uint id, string line, string value, ProvinceDatabase database)
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
                database.AddStateBuilding(id, tempStateBuilding);
                break;
        }
    }

    public void Dispose()
    {
        isReadingBuildings = false;
        tempStateBuilding = new StateBuilding();
    }
}