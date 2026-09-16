using Paradox_Editor.Extensions;
using System.Collections.Generic;
using System.IO;

namespace Paradox_Editor.Parsers;

public sealed class ContinentParser : ParserCommon
{
    public override T Parse<T>(string directory, params string[] fileParts)
    {
        Dictionary<string, Continent> continents = new();

        var continentFilePath = GetGameFilePath(directory, Path.Combine(fileParts));
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
            switch (item)
            {
                case "assimilation_rate":
                    reader.SkipUntil('=');
                    reader.SkipWhitespace();
                    continent.AssimilationRate = reader.ReadUntilWhitespace();
                    break;
                case "farm_rgo_size":
                    reader.SkipUntil('=');
                    reader.SkipWhitespace();
                    continent.RGOSizeFarm = reader.ReadUntilWhitespace();
                    break;
                case "mine_rgo_size":
                    reader.SkipUntil('=');
                    reader.SkipWhitespace();
                    continent.RGOSizeMine = reader.ReadUntilWhitespace();
                    break;
                default:
                    continent.Provinces = ReadProvinces(reader);
                    break;
            }

            reader.SkipWhitespace();
        }

        reader.Read();
        return continent;
    }

    private static List<string> ReadProvinces(StreamReader reader)
    {
        List<string> provIDs = [];

        reader.SkipUntil('{');
        reader.SkipWhitespace();
        while (reader.Peek() is not -1 and not '}')
        {
            if (reader.Peek() == '\"')
            {
                reader.Read();
                provIDs.Add(reader.ReadUntil('\"'));
            }
            else
            {
                provIDs.Add(reader.ReadUntilWhitespace());
            }

            reader.SkipWhitespace();
        }

        reader.Read();
        return provIDs; //test*/
    }
}