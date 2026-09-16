using System.IO;
using System.Windows.Media;
using Paradox_Editor.Extensions;
using Paradox_Editor.Parsers;

namespace Paradox_Editor.Types;

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
                    good.Color = ColorParser.Parse(reader);
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