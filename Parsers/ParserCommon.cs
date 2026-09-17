using System.IO;
using Paradox_Editor.Extensions;

namespace Paradox_Editor.Parsers;

public abstract class ParserCommon(string Directory)
{
    public bool IsIncludedInMod { get; set; }

    public abstract T Parse<T>(params string[] fileParts) where T : new();

    /// <summary>
    /// Obtains a game file from a provided mod directory and file name (that which requires path combination), but
    /// will also default to the game directory if the mod does not contain the file.
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    protected string GetGameFilePath(string file)
    {
        // Find the file from vanilla if it's not overwritten.
        bool pathIsLocalToMod = GetGameFilePath(Directory, file, out var path);
        IsIncludedInMod = pathIsLocalToMod;
        return path;
    }

    internal static bool GetGameFilePath(string directory, string fileName, out string path)
    {
        string gamePath = Path.Combine(directory, fileName);
        if (Utilities.FileIsInMod(fileName))
        {
            path = gamePath;
            return true;
        }

        // Find the file from vanilla if it's not overwritten.
        path = Path.Combine(ModData.Instance.MOD_DATA.GetGameDirectory(), fileName);
        return false;
    }
}