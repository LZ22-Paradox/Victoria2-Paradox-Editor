using System;
using System.IO;
using Paradox_Editor.Extensions;
using Paradox_Editor.Types;
using Paradox_Editor.Types.Data;

namespace Paradox_Editor.Parsers;

public class DefaultMapParser(string Directory) : ParserCommon(Directory)
{
    public override T Parse<T>(params string[] fileParts)
    {
        var defaultMap = new DefaultMap();
        var path = GetGameFilePath(Path.Combine(fileParts));
        using var reader = new StreamReader(File.OpenRead(path));
        while (!reader.EndOfStream)
        {
            reader.SkipWhitespace();
            if (reader.EndOfStream)
                break;

            ReadProperty(reader, defaultMap);
        }

        return (T)(object)defaultMap;
    }

    private static void ReadProperty(StreamReader reader, DefaultMap map)
    {
        while (true)
        {
            reader.SkipWhitespace();
            if (reader.Peek() is -1 or '}')
                break;

            string key = reader.ReadUntilWhitespace();
            switch (key)
            {
                case "max_provinces":
                    map.MaxProvinces = Convert.ToInt32(reader.ReadAssignmentValue());
                    break;
                case "sea_starts":
                    reader.SkipUntil('=');
                    map.SeaStarts = reader.ReadIntList();
                    break;
                case "definitions":
                    map.DefinitionsPath = reader.ReadAssignmentValue();
                    break;
                case "provinces":
                    map.ProvincesMap = reader.ReadAssignmentValue();
                    break;
                case "positions": // The positions text file. Not actual province positions.
                    map.PositionsMap = reader.ReadAssignmentValue();
                    break;
                case "terrain":
                    map.TerrainMap = reader.ReadAssignmentValue();
                    break;
                case "rivers":
                    map.RiversMap = reader.ReadAssignmentValue();
                    break;
                case "terrain_definition":
                    map.TerrainDefinitions = reader.ReadAssignmentValue();
                    break;
                case "tree_definition":
                    map.TreeDefinitions = reader.ReadAssignmentValue();
                    break;
                case "continent":
                    map.ContinentFile = reader.ReadAssignmentValue();
                    break;
                case "adjacencies":
                    map.AdjacenciesFile = reader.ReadAssignmentValue();
                    break;
                case "region":
                    map.RegionFile = reader.ReadAssignmentValue();
                    break;
                case "region_sea":
                    map.SeaRegionsFile = reader.ReadAssignmentValue();
                    break;
                case "province_flag_sprite":
                    map.ProvinceFlagSpritesFile = reader.ReadAssignmentValue();
                    break;
                case "border_heights":
                    reader.SkipUntil('=');
                    map.BorderHeights = reader.ReadIntList().ToArray();
                    break;
                case "terrain_sheet_heights":
                    reader.SkipUntil('=');
                    map.TerrainSheetHeights = reader.ReadIntList().ToArray();
                    break;
                case "tree":
                    map.TreeValue = Convert.ToInt32(reader.ReadAssignmentValue());
                    break;
                case "border_cutoff":
                    map.BorderCutoffValue = float.Parse(reader.ReadAssignmentValue());
                    break;
            }

            reader.Read();
        }
    }
}