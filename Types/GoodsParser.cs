using System.Collections.Generic;
using System.IO;
using Paradox_Editor.Extensions;
using Paradox_Editor.Parsers;

namespace Paradox_Editor.Types;

public class GoodsParser : ParserCommon
{
    public override T Parse<T>(string directory, params string[] fileParts)
    {
        Dictionary<string, GoodGroup> goods = new();
        var goodsPath = GetGameFilePath(directory, Path.Combine(fileParts));
        using StreamReader reader = new(File.OpenRead(goodsPath));
        while (true)
        {
            if (reader.Peek() == -1)
                break;

            reader.SkipWhitespace();
            string name = reader.ReadUntil(' ');
            if (!string.IsNullOrWhiteSpace(name))
            {
                goods[name] = GoodGroup.Parse(reader);
            }
        }

        return (T)(object)goods;
    }
}