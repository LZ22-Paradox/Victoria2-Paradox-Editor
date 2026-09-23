using System.Collections.Generic;
using System.IO;
using Paradox_Editor.Extensions;

namespace Paradox_Editor.Types;

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
                    group.Ideologies[item] = Ideology.ReadIdeology(reader);
                    break;
            }

            reader.SkipWhitespace();
        }

        reader.Read();
        return group;
    }
}