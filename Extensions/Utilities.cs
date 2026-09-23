using System.IO;
using System.Linq;
using System.Windows.Media;

namespace Paradox_Editor.Extensions;

public static class Utilities
{
    public static string ModDirectory = "";

    public static string RemoveWhitespace(this string input)
        => new(input.ToCharArray().Where(c => !char.IsWhiteSpace(c)).ToArray());

    public static bool FileIsInMod(string file) => File.Exists(Path.Combine(ModDirectory, file));

    public static uint ToPackedColor(this Color color)
        => (0xFFu << 24) | ((uint)color.R << 16) | ((uint)color.G << 8) | color.B;

    public static Color UnpackAsArgbColor(this uint color)
        => Color.FromArgb((byte)(color >> 24), (byte)(color >> 16), (byte)(color >> 8), (byte)color);
}