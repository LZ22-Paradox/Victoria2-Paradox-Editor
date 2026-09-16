using Paradox_Editor.Extensions;
using System;
using System.Collections.Generic;
using System.IO;

namespace Paradox_Editor.Parsers;

public sealed class CultureParser
{
    public static Dictionary<string, CultureGroup> Parse(string directory)
    {
        Dictionary<string, CultureGroup> groups = new();
        string culturesCommonFilePath = Path.Combine(directory, "common", "cultures.txt"); //Find if overwritten

        CultureParser file = new();
        using StreamReader reader = new(File.OpenRead(culturesCommonFilePath));
        while (true)
        {
            int c = reader.Peek();
            if (c == -1)
            {
                break;
            }
            reader.SkipWhitespace();
            string name = reader.ReadUntil(' ');
            groups[name] = CultureGroup.Parse(reader);
        }
        return groups;
    }
}

public sealed class CultureGroup
{
    public string Leader { get; set; }
    public string Unit { get; set; }
    public string IsOverseas { get; set; }
    public Dictionary<string, Culture> Cultures { get; } = new();
    public string Union { get; set; }

    public static CultureGroup Parse(StreamReader reader)
    {
        CultureGroup group = new();
        reader.SkipUntil('{');
        reader.SkipWhitespace();
        while (reader.Peek() is not -1 and not '}')
        {
            string item = reader.ReadUntilWhitespace();
            switch (item)
            {
                case "leader":
                    reader.SkipUntil('=');
                    reader.SkipWhitespace();
                    group.Leader = reader.ReadUntilWhitespace();
                    break;
				case "is_overseas":
					reader.SkipUntil('=');
					reader.SkipWhitespace();
					group.IsOverseas = reader.ReadUntilWhitespace();
					break;
				case "unit":
                    reader.SkipUntil('=');
                    reader.SkipWhitespace();
                    group.Unit = reader.ReadUntilWhitespace();
                    break;
                case "union":
                    reader.SkipUntil('=');
                    reader.SkipWhitespace();
                    group.Union = reader.ReadUntilWhitespace();
                    break;
                default:
                    group.Cultures[item] = Culture.Parse(reader);
                    break;
            }
            reader.SkipWhitespace();
        }
        reader.Read();
        return group;
    }
}

public sealed class Culture
{
    public Color Color { get; set; }
    public List<string> FirstNames { get; } = new();
    public List<string> LastNames { get; } = new();

    public static Culture Parse(StreamReader reader)
    {
        Culture culture = new();
        reader.SkipUntil('{');
        reader.SkipWhitespace();
        while (reader.Peek() is not -1 and not '}')
        {
            string item = reader.ReadUntilWhitespace();
            switch (item)
            {
                case "color":
                    culture.Color = Color.Parse(reader);
                    break;
                case "first_names":
                    reader.SkipUntil('{');
                    reader.SkipWhitespace();
                    while (reader.Peek() is not -1 and not '}')
                    {
                        if (reader.Peek() == '\"')
                        {
                            reader.Read();
                            culture.FirstNames.Add(reader.ReadUntil('\"'));
                        }
                        else
                        {
                            culture.FirstNames.Add(reader.ReadUntilWhitespace());
                        }
                        reader.SkipWhitespace();
                    }
                    reader.Read();
                    break;
                case "last_names":
                    reader.SkipUntil('{');
                    reader.SkipWhitespace();
                    while (reader.Peek() is not -1 and not '}')
                    {
                        if (reader.Peek() == '\"')
                        {
                            reader.Read();
                            culture.LastNames.Add(reader.ReadUntil('\"'));
                        }
                        else
                        {
                            culture.LastNames.Add(reader.ReadUntilWhitespace());
                        }
                        reader.SkipWhitespace();
                    }
                    reader.Read();
                    break;
            }
            reader.SkipWhitespace();
        }
        reader.Read();
        return culture;
    }
}

public record struct Color(byte r, byte b, byte g)
{
    public static Color Parse(StreamReader reader)
    {
        reader.SkipUntil('{');
        Span<byte> nums = stackalloc byte[3];
        for (int i = 0; i < nums.Length; i++)
        {
            reader.SkipWhitespace();
            nums[i] = byte.Parse(reader.ReadUntil(' ', '}'));
        }
        reader.SkipUntil('}');
        return new Color(nums[0], nums[1], nums[2]);
    }
}