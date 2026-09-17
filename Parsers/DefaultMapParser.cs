using System.IO;
using Paradox_Editor.Extensions;

namespace Paradox_Editor.Parsers;

public class DefaultMapParser(string Directory) : ParserCommon(Directory)
{
    public override T Parse<T>(params string[] fileParts)
    {
        T defaultMap = new();
        var path = GetGameFilePath(Path.Combine(fileParts));
        using StreamReader reader = new(File.OpenRead(path));
        while (true)
        {
            int nextCharacter = reader.Peek();
            if (nextCharacter == -1) break;

            reader.SkipWhitespace();
            string line = reader.ReadUntil(' ');
            if (string.IsNullOrWhiteSpace(line))
                continue;
            // WIP: FINISH THIS
        }
        
        
        return defaultMap;
    }

    public void ReadDefaultMap()
    {
        
    }
}