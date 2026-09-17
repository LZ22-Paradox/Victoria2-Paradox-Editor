using Paradox_Editor.Extensions;
using System.Collections.Generic;
using System.IO;
using Paradox_Editor.Types;

namespace Paradox_Editor.Parsers;

public sealed class CultureParser(string Directory) : ParserCommon(Directory)
{
    public override T Parse<T>(params string[] fileParts)
    {
        Dictionary<string, CultureGroup> groups = new();
        string culturesCommonFilePath = GetGameFilePath(Path.Combine(fileParts));
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
            groups[name] = ReadCultureGroup(reader);
        }

        return (T)(object)groups;
    }
    

    
    private static CultureGroup ReadCultureGroup(StreamReader reader)
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
                    group.Cultures[item] = ReadCulture(reader);
                    break;
            }

            reader.SkipWhitespace();
        }

        reader.Read();
        return group;
    }

    private static Culture ReadCulture(StreamReader reader)
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
                    culture.Color = ColorParser.Parse(reader);
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