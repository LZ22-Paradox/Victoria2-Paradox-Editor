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

    // ReSharper disable once RedundantCast
    public static uint ToPackedColor(this Color color)
        => (0xFFu << 24) | ((uint)color.R << 16) | ((uint)color.G << 8) | (uint)color.B;
    

}