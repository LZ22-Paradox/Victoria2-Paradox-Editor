using System.IO;
using Paradox_Editor.Extensions;

namespace Paradox_Editor.Parsers;

public abstract class ParserCommon
{
    public bool IsIncludedInMod { get; set; }

    public abstract T Parse<T>(string directory, params string[] fileParts) where T : new();

    /// <summary>
    /// Obtains a game file from a provided mod directory and file name (that which requires path combination), but
    /// will also default to the game directory if the mod does not contain the file.
    /// </summary>
    /// <param name="directory"></param>
    /// <param name="fileName"></param>
    /// <returns></returns>
    protected string GetGameFilePath(string directory, string fileName)
    {
        string continentFilePath = Path.Combine(directory, fileName);
        if (Utilities.FileIsInMod(fileName))
        {
            IsIncludedInMod = true;
            return continentFilePath;
        }

        // Find the file from vanilla if it's not overwritten.
        IsIncludedInMod = false;
        continentFilePath = Path.Combine(ModData.Instance.MOD_DATA.GetGameDirectory(), fileName);
        return continentFilePath;
    }
}