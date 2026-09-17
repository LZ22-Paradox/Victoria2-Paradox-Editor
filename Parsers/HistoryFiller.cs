using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Paradox_Editor.Types;
using Paradox_Editor.Types.Data;

namespace Paradox_Editor.Parsers;

public class HistoryFiller
{
    public static void PopulateProvinceHistory(uint provinceId, string historyFilePath, DatabaseProvinces provinceDatabase)
    {
        List<Tag> cores = [];

        using StreamReader reader = new(historyFilePath);

        while (reader.ReadLine() is { } rawLine)
        {
            string line = RemoveComment(rawLine).Trim();

            if (string.IsNullOrWhiteSpace(line))
                continue;

            // Ignore standalone closing braces.
            if (line == "}")
                continue;

            int equalsIndex = line.IndexOf('=');

            if (equalsIndex < 0)
                continue;

            string key = line[..equalsIndex].Trim();
            string value = line[(equalsIndex + 1)..].Trim();

            // ------------------------------------------------------------
            // DATED HISTORY
            //
            // Example:
            //
            // 1836.1.11 = {
            //     owner = ENG
            //     controller = ENG
            //     add_core = ENG
            // }
            // ------------------------------------------------------------

            if (TryParseHistoryDate(key, out DateTime date))
            {
                if (value != "{")
                    continue;

                ProvinceHistoryEvent historyEvent =
                    ParseHistoryEvent(reader, date);

                provinceDatabase.AddHistoryEvent(provinceId, historyEvent);

                continue;
            }

            // ------------------------------------------------------------
            // STATE BUILDING
            //
            // Example:
            //
            // state_building = {
            //     building = railroad
            //     level = 1
            // }
            //
            // This is an undated/current province value, so it still
            // gets added directly to the province database.
            // ------------------------------------------------------------

            if (key.Equals("state_building", StringComparison.Ordinal))
            {
                StateBuilding? stateBuilding =
                    ParseStateBuilding(reader, value);

                if (stateBuilding != null)
                    provinceDatabase.AddStateBuilding(provinceId, stateBuilding);

                continue;
            }

            // ------------------------------------------------------------
            // NORMAL / UNDATED HISTORY
            // ------------------------------------------------------------

            ParseCurrentHistoryLine(
                provinceId,
                key,
                value,
                cores,
                provinceDatabase);
        }

        provinceDatabase.SetCores(provinceId, cores);
    }

    // ====================================================================
    // DATED HISTORY
    // ====================================================================

    private static ProvinceHistoryEvent ParseHistoryEvent(
        StreamReader reader,
        DateTime date)
    {
        ProvinceHistoryEvent historyEvent = new()
        {
            Date = date
        };

        while (reader.ReadLine() is { } rawLine)
        {
            string line = RemoveComment(rawLine).Trim();

            if (string.IsNullOrWhiteSpace(line))
                continue;

            // End of the dated history block.
            if (line == "}")
                break;

            int equalsIndex = line.IndexOf('=');

            if (equalsIndex < 0)
                continue;

            string key = line[..equalsIndex].Trim();
            string value = line[(equalsIndex + 1)..].Trim();

            // ------------------------------------------------------------
            // Nested state building
            // ------------------------------------------------------------

            if (key.Equals("state_building", StringComparison.Ordinal))
            {
                StateBuilding? stateBuilding =
                    ParseStateBuilding(reader, value);

                if (stateBuilding != null)
                    historyEvent.StateBuildings.Add(stateBuilding);

                continue;
            }

            // ------------------------------------------------------------
            // Dated province fields
            // ------------------------------------------------------------

            ParseEventHistoryLine(
                key,
                value,
                historyEvent);
        }

        return historyEvent;
    }

    private static void ParseEventHistoryLine(
        string key,
        string value,
        ProvinceHistoryEvent historyEvent)
    {
        value = value.Trim();

        switch (key)
        {
            case "owner":
                historyEvent.Owner = value;
                break;

            case "controller":
                historyEvent.Controller = value;
                break;

            case "trade_goods":
                historyEvent.TradeGoods = value;
                break;

            case "life_rating":
                if (short.TryParse(
                        value,
                        NumberStyles.Integer,
                        CultureInfo.InvariantCulture,
                        out short lifeRating))
                {
                    historyEvent.LifeRating = lifeRating;
                }

                break;

            case "colonial":
                historyEvent.Colonial = ParseBoolean(value);
                break;

            case "terrain":
                historyEvent.Terrain = value;
                break;

            case "add_core":
                if (!string.IsNullOrWhiteSpace(value))
                    historyEvent.AddedCores.Add(new Tag(value));

                break;

            case "naval_base":
                if (short.TryParse(
                        value,
                        NumberStyles.Integer,
                        CultureInfo.InvariantCulture,
                        out short navalBase))
                {
                    historyEvent.NavalBase = navalBase;
                }

                break;

            case "fort":
                if (short.TryParse(
                        value,
                        NumberStyles.Integer,
                        CultureInfo.InvariantCulture,
                        out short fort))
                {
                    historyEvent.Fort = fort;
                }

                break;

            case "railroad":
                if (short.TryParse(
                        value,
                        NumberStyles.Integer,
                        CultureInfo.InvariantCulture,
                        out short railroad))
                {
                    historyEvent.Railroad = railroad;
                }

                break;
        }
    }

    // ====================================================================
    // CURRENT / UNDATED HISTORY
    // ====================================================================

    private static void ParseCurrentHistoryLine(
        uint id,
        string key,
        string value,
        List<Tag> cores,
        DatabaseProvinces database)
    {
        value = value.Trim();

        switch (key)
        {
            case "owner":
                database.SetOwner(id, value);
                break;

            case "controller":
                database.SetController(id, value);
                break;

            case "trade_goods":
                database.SetTradeGood(id, value);
                break;

            case "life_rating":
                if (short.TryParse(
                        value,
                        NumberStyles.Integer,
                        CultureInfo.InvariantCulture,
                        out short lifeRating))
                {
                    database.SetLifeRating(id, lifeRating);
                }

                break;

            case "colonial":
                database.SetColonial(id, ParseBoolean(value));
                break;

            case "terrain":
                database.SetTerrain(id, value);
                break;

            case "add_core":
                if (!string.IsNullOrWhiteSpace(value))
                    cores.Add(new Tag(value));

                break;

            case "naval_base":
                if (short.TryParse(
                        value,
                        NumberStyles.Integer,
                        CultureInfo.InvariantCulture,
                        out short navalBase))
                {
                    database.SetNavalBaseLevel(id, navalBase);
                }

                break;

            case "fort":
                if (short.TryParse(
                        value,
                        NumberStyles.Integer,
                        CultureInfo.InvariantCulture,
                        out short fort))
                {
                    database.SetFortLevel(id, fort);
                }

                break;

            case "railroad":
                if (short.TryParse(
                        value,
                        NumberStyles.Integer,
                        CultureInfo.InvariantCulture,
                        out short railroad))
                {
                    database.SetRailroadLevel(id, railroad);
                }

                break;
        }
    }

    // ====================================================================
    // STATE BUILDINGS
    // ====================================================================

    private static StateBuilding? ParseStateBuilding(
        StreamReader reader,
        string value)
    {
        value = value.Trim();

        StateBuilding stateBuilding = new();

        // Normal form:
        //
        // state_building = {
        //
        if (value == "{")
        {
            while (reader.ReadLine() is { } rawLine)
            {
                string line = RemoveComment(rawLine).Trim();

                if (string.IsNullOrWhiteSpace(line))
                    continue;

                if (line == "}")
                    break;

                ParseStateBuildingLine(
                    line,
                    stateBuilding);
            }

            return stateBuilding;
        }

        // Handles:
        //
        // state_building = { building = railroad ... }
        //
        // although the normal Paradox files generally use the multiline
        // form above.
        value = value.TrimStart('{').Trim();

        if (!string.IsNullOrWhiteSpace(value))
        {
            ParseStateBuildingLine(
                value,
                stateBuilding);
        }

        return stateBuilding;
    }

    private static void ParseStateBuildingLine(
        string line,
        StateBuilding stateBuilding)
    {
        int equalsIndex = line.IndexOf('=');

        if (equalsIndex < 0)
            return;

        string key = line[..equalsIndex].Trim();
        string value = line[(equalsIndex + 1)..].Trim();

        switch (key)
        {
            case "building":
                stateBuilding.Building = value;
                break;

            case "level":
                stateBuilding.Level = value;
                break;

            case "upgrade":
                stateBuilding.Upgrade = value;
                break;
        }
    }

    // ====================================================================
    // DATE PARSING
    // ====================================================================

    private static bool TryParseHistoryDate(
        string value,
        out DateTime date)
    {
        return DateTime.TryParseExact(
            value,
            "yyyy.M.d",
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out date);
    }

    // ====================================================================
    // BOOLEAN
    // ====================================================================

    private static short ParseBoolean(string value)
    {
        if (short.TryParse(
                value,
                NumberStyles.Integer,
                CultureInfo.InvariantCulture,
                out short result))
        {
            return result;
        }

        return value.Equals(
            "yes",
            StringComparison.OrdinalIgnoreCase)
            ? (short)1
            : (short)0;
    }

    // ====================================================================
    // COMMENTS
    // ====================================================================

    private static string RemoveComment(string line)
    {
        int commentIndex = line.IndexOf('#');

        return commentIndex >= 0
            ? line[..commentIndex]
            : line;
    }
}