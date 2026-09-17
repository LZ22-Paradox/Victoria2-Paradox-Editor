using Paradox_Editor.Parsers;

namespace Paradox_Editor.DataAcquisition;

public class LocalizationAcquisition(string Directory) : ParserCommon(Directory)
{
    // TODO: Add Localization support.
    public override T Parse<T>(params string[] fileParts)
    {
        throw new System.NotImplementedException();
    }
}