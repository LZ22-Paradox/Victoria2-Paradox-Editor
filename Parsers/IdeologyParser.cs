using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Paradox_Editor.Extensions;

namespace Paradox_Editor.Parsers;

public sealed class IdeologiesFile // TODO: This all needs work.
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
                break;

            reader.SkipWhitespace();
            string name = reader.ReadUntil(' ');
            file.Groups[name] = IdeologyGroup.Parse(reader);
        }

        return file;
    }
    
    public static List<Ideology> Read(StreamReader reader)
    {
        List<Ideology> ideologyList = [];
        // TODO: Go through this.
        /*reader.SkipUntil('{');
        reader.SkipWhitespace();
        while (reader.Peek() is not -1 and not '}')
        {
            Ideology ideology = new();
            string? line = reader.ReadLine();
            var regex = new Regex("\t|\\s+");
            if (!string.IsNullOrEmpty(line))
            {
                string[] cleanedLine = regex.Replace(line, "").Split('=');
                ideology.Name = cleanedLine[0];
                ideology.Percentage = cleanedLine[1];
            }

            ideologyList.Add(ideology);
            reader.SkipWhitespace();
        }

        reader.Read();*/
        return ideologyList;
    }
}