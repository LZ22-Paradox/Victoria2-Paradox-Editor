using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Paradox_Editor.Extensions;

namespace Paradox_Editor.Ideologys;

public sealed class IdeologiesFile
{
    /*
     * ADD REFORM TYPES
     * - Modifiers
     *      o Contain Factors
     *      o Ruling Party Ideology
     *      o Militancy
     *      
     *          * May Need To Look At Translating Vic2's Modifiers
     * - Base (Some Integer Value)
     */

    public Dictionary<string, IdeologyGroup> Groups { get; } = new();

    public static IdeologiesFile Parse(string path)
    {
        IdeologiesFile file = new();
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
            file.Groups[name] = IdeologyGroup.Parse(reader);
        }
        return file;
    }
}

public sealed class IdeologyGroup
{
    //public string Unit { get; set; }
    public Dictionary<string, Ideology> Ideologies { get; } = new();

    public static IdeologyGroup Parse(StreamReader reader)
    {
        IdeologyGroup group = new();
        reader.SkipUntil('{');
        reader.SkipWhitespace();
        while (reader.Peek() is not -1 and not '}')
        {
            string item = reader.ReadUntilWhitespace();
            switch (item)
            {
                /*case "unit":
                    reader.SkipUntil('=');
                    reader.SkipWhitespace();
                    group.Unit = reader.ReadUntilWhitespace();
                    break;*/
                default:
                    group.Ideologies[item] = Ideology.Parse(reader);
                    break;
            }
            reader.SkipWhitespace();
        }
        reader.Read();
        return group;
    }
}

public sealed class Ideology
{
    public Color Color { get; set; }
    public string CanReduceMilitary { get; set; }
    public string Date { get; set; }
    public string Uncivilized { get; set; }

    public List<string> FirstNames { get; } = new();
    public List<string> LastNames { get; } = new();

    public static Ideology Parse(StreamReader reader)
    {
        Ideology culture = new();
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