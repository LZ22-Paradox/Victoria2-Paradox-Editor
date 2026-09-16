using System.IO;
using Paradox_Editor.Extensions;
using Paradox_Editor.Types;

namespace Paradox_Editor.Parsers;

public class DefaultMapParser : ParserCommon
{
    public override T Parse<T>(string directory, params string[] fileParts)
    {
        T defaultMap = new();
    
        var path = GetGameFilePath(directory, Path.Combine(fileParts));
        using StreamReader reader = new(File.OpenRead(path));
        while (true)
        {
            int nextCharacter = reader.Peek();
            if (nextCharacter == -1) break;

            reader.SkipWhitespace();
            string line = reader.ReadUntil(' ');
            if (string.IsNullOrWhiteSpace(line))
                continue;
            
        }
        
        
        return defaultMap;
    }

    public void ReadDefaultMap()
    {
        
    }
}