using Paradox_Editor.Parsers;

namespace Paradox_Editor.DataAcquisition;

public class LocalizationAcquisition : ParserCommon
{
    public LocalizationAcquisition(string directory)
    {
        // TODO: Add Localization support.
    }

    public override T Parse<T>(string directory, params string[] fileParts)
    {
        throw new System.NotImplementedException();
    }
}