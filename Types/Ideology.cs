using System.Collections.Generic;
using System.IO;
using Paradox_Editor.Extensions;

namespace Paradox_Editor.Parsers;

public sealed class Ideology
{
    public System.Windows.Media.Color Color { get; set; }
    public string CanReduceMilitary { get; set; }
    public string Date { get; set; }
    public string Uncivilized { get; set; }

    public List<string> FirstNames { get; } = new();
    public List<string> LastNames { get; } = new();

    public static Ideology ReadIdeology(StreamReader reader)
    {
        Ideology ideology = new();
        reader.SkipUntil('{');
        reader.SkipWhitespace();
        while (reader.Peek() is not -1 and not '}')
        {
            string item = reader.ReadUntilWhitespace();
            switch (item)
            {
                case "color":
                    ideology.Color = ColorParser.Parse(reader);
                    break;
                case "first_names":
                    reader.SkipUntil('{');
                    reader.SkipWhitespace();
                    while (reader.Peek() is not -1 and not '}')
                    {
                        if (reader.Peek() == '\"')
                        {
                            reader.Read();
                            ideology.FirstNames.Add(reader.ReadUntil('\"'));
                        }
                        else
                        {
                            ideology.FirstNames.Add(reader.ReadUntilWhitespace());
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
                            ideology.LastNames.Add(reader.ReadUntil('\"'));
                        }
                        else
                        {
                            ideology.LastNames.Add(reader.ReadUntilWhitespace());
                        }

                        reader.SkipWhitespace();
                    }

                    reader.Read();
                    break;
            }

            reader.SkipWhitespace();
        }

        reader.Read();
        return ideology;
    }
}