using Paradox_Editor.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Paradox_Editor.Cultures;

public sealed class ContinentParser
{
    public static Dictionary<string, Continent> Parse(string directory)
    {
        Dictionary<string, Continent> continents = new();
		string continentFilePath = Path.Combine(directory, "map", "continent.txt"); //Find if overwritten

        using StreamReader reader = new(File.OpenRead(continentFilePath));
        while (true)
        {
            int c = reader.Peek();
            if (c == -1)
            {
                break;
            }
            reader.SkipWhitespace();
            string name = reader.ReadUntil(' ');
            if (!string.IsNullOrWhiteSpace(name))
            {
                continents[name] = Continent.Parse(reader);
            }
        }
        return continents;
    }
}

public sealed class Continent
{
    public List<string> Provinces { get; set; } = new();
	public string assimilation_rate;
	public string farm_rgo_size;
	public string mine_rgo_size;

    public static Continent Parse(StreamReader reader)
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
                    continent.assimilation_rate = reader.ReadUntilWhitespace();
                    break;
                case "farm_rgo_size":
                    reader.SkipUntil('=');
                    reader.SkipWhitespace();
                    continent.farm_rgo_size = reader.ReadUntilWhitespace();
                    break;
                case "mine_rgo_size":
                    reader.SkipUntil('=');
                    reader.SkipWhitespace();
                    continent.mine_rgo_size = reader.ReadUntilWhitespace();
                    break;
                default:
                    continent.Provinces = ParseProvinces(reader);
                    break;
            }
            reader.SkipWhitespace();
        }
        reader.Read();
        return continent;
    }

	private static List<string> ParseProvinces(StreamReader reader)
	{
		List<string> provIDs = new();

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

