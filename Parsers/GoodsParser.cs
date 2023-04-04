using Paradox_Editor.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Paradox_Editor.Parsers;

public sealed class GoodsParser
{
    public static Dictionary<string, GoodGroup> Parse(string directory)
    {
        Dictionary<string, GoodGroup> goods = new();
        string continentFilePath = Path.Combine(directory, "common", "goods.txt"); //Find if overwritten

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
                goods[name] = GoodGroup.Parse(reader);
            }
        }
        return goods;
    }
}

public sealed class GoodGroup
{
	public Dictionary<string, Good> Goods { get; } = new();
	public static GoodGroup Parse(StreamReader reader)
	{
		GoodGroup group = new();
		reader.SkipUntil('{');
		reader.SkipWhitespace();
		while (reader.Peek() is not -1 and not '}')
		{
			string item = reader.ReadUntilWhitespace();
			switch (item)
			{
				/*case "leader":
					reader.SkipUntil('=');
					reader.SkipWhitespace();
					group.Leader = reader.ReadUntilWhitespace();
					break;*/
				default:
					group.Goods[item] = Good.Parse(reader);
					break;
			}
			reader.SkipWhitespace();
		}
		reader.Read();
		return group;
	}
}

public sealed class Good
{
	public double Cost { get; set; }
	public Color Color { get; set; }
	public string AvailableFromStart { get; set; }

	public static Good Parse(StreamReader reader)
	{
		Good good = new();
		reader.SkipUntil('{');
		reader.SkipWhitespace();
		while (reader.Peek() is not -1 and not '}')
		{
			string item = reader.ReadUntilWhitespace();
			switch (item)
			{
				case "cost":
					reader.SkipUntil('=');
					reader.SkipWhitespace();
					good.Cost = double.Parse(reader.ReadUntilWhitespace());
					break;
				case "color":
					good.Color = Color.Parse(reader);
					break;
				case "available_from_start":
					reader.SkipUntil('=');
					reader.SkipWhitespace();
					good.AvailableFromStart = reader.ReadUntilWhitespace();
					break;
		}
			reader.SkipWhitespace();
		}
		reader.Read();
		return good;
	}
}



