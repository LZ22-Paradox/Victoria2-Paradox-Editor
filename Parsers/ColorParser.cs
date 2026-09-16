using System;
using System.IO;
using Paradox_Editor.Extensions;

namespace Paradox_Editor.Parsers;

public static class ColorParser
{
    public static System.Windows.Media.Color Parse(StreamReader reader)
    {
        reader.SkipUntil('{');
        Span<byte> nums = stackalloc byte[3];
        for (int i = 0; i < nums.Length; i++)
        {
            reader.SkipWhitespace();
            nums[i] = byte.Parse(reader.ReadUntil(' ', '}'));
        }

        reader.SkipUntil('}');
        return System.Windows.Media.Color.FromRgb(nums[0], nums[1], nums[2]);
    }
}