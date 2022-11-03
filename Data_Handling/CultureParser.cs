using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Paradox_Editor.Cultures;

public sealed class CulturesFile
{
    public Dictionary<string, CultureGroup> Groups { get; } = new();

    public static CulturesFile Parse(string path)
    {
        CulturesFile file = new();
        using StreamReader reader = new(File.OpenRead(path));
        while (true)
        {
            int c = reader.Peek();
            if (c == -1)
            {
                break;
            }
            reader.SkipWhitespace();
            string name = reader.ReadUntil(' ');
            file.Groups[name] = CultureGroup.Parse(reader);
        }
        return file;
    }
}

public sealed class CultureGroup
{
    public string Leader { get; set; }
    public string Unit { get; set; }
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
        return new(nums[0], nums[1], nums[2]);
    }
}

public static class ReaderExtensions
{
    public static string ReadUntil(this StreamReader reader, char end)
    {
        StringBuilder builder = new();
        int c;
        while ((c = reader.Read()) != -1 && (char)c != end)
        {
            builder.Append((char)c);
        }
        return builder.ToString();
    }
    public static string ReadUntil(this StreamReader reader, params char[] end)
    {
        StringBuilder builder = new();
        int c;
        while ((c = reader.Read()) != -1 && !end.Contains((char)c))
        {
            builder.Append((char)c);
        }
        return builder.ToString();
    }
    public static string ReadUntilWhitespace(this StreamReader reader)
    {
        StringBuilder builder = new();
        while (true)
        {
            int c = reader.Read();
            if (c == -1)
            {
                return builder.ToString();
            }
            if (c == '#')
            {
                reader.SkipUntil('\n');
            }
            if (c is ' ' or '\n' or '\t' or '\r')
            {
                return builder.ToString();
            }
            builder.Append((char)c);
        }
    }
    public static void SkipUntil(this StreamReader reader, char end)
    {
        while (true)
        {
            int c = reader.Read();
            if (c == -1 || (char)c == end) return;
        }
    }
    public static void SkipWhitespace(this StreamReader reader)
    {
        while (true)
        {
            int c = reader.Peek();
            if (c == -1)
            {
                return;
            }
            if (c == '#')
            {
                reader.SkipUntil('\n');
            }
            else if (c is ' ' or '\n' or '\t' or '\r')
            {
                reader.Read();
            }
            else
            {
                return;
            }
        }
    }
}
