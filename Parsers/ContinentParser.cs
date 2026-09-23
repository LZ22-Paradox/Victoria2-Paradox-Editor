using Paradox_Editor.Extensions;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Paradox_Editor.Types;

namespace Paradox_Editor.Parsers;

public sealed class ContinentParser(string Directory) : ParserCommon(Directory)
{
    public override T Parse<T>(params string[] fileParts)
    {
        Dictionary<string, Continent> continents = new();
        var continentFilePath = GetGameFilePath(Path.Combine(fileParts));
        using StreamReader reader = new(File.OpenRead(continentFilePath));
        while (true)
        {
            int nextCharacter = reader.Peek();
            if (nextCharacter == -1) break;

            reader.SkipWhitespace();
            string name = reader.ReadUntil(' ');
            if (!string.IsNullOrWhiteSpace(name))
                continents[name] = ReadContinent(reader);
        }

        return (T)(object)continents;
    }

    private static Continent ReadContinent(StreamReader reader)
    {
        Continent continent = new();
        reader.SkipUntil('{');
        reader.SkipWhitespace();
        while (reader.Peek() is not -1 and not '}')
        {
            string item = reader.ReadUntilWhitespace();
            reader.SkipUntil('=');
            reader.SkipWhitespace();
            switch (item)
            {
                case "assimilation_rate":
                    continent.AssimilationRate = reader.ReadUntilWhitespace();
                    break;
                case "farm_rgo_size":
                    continent.RGOSizeFarm = reader.ReadUntilWhitespace();
                    break;
                case "mine_rgo_size":
                    continent.RGOSizeMine = reader.ReadUntilWhitespace();
                    break;
                default: // Defaults to "provinces"
                    var provinces = reader.ReadList(ParseUInt).Cast<uint>().ToList();
                    continent.Provinces = provinces;
                    break;
            }

            reader.SkipWhitespace();
        }

        reader.Read();
        return continent;
    }

    private static uint? ParseUInt(string value)
    {
        if (!uint.TryParse(value, out uint result)) return null;
        return result;
    }
}