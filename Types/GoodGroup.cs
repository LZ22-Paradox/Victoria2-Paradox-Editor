using System.Collections.Generic;
using System.IO;
using Paradox_Editor.Extensions;

namespace Paradox_Editor.Types;

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
            group.Goods[item] = item switch { _ => Good.Parse(reader) };
            reader.SkipWhitespace();
        }

        reader.Read();
        return group;
    }
}