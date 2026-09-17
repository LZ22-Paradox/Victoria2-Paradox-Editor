using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Paradox_Editor.Parsers;
using Paradox_Editor.Types;

namespace Paradox_Editor.DataAcquisition;

public static class HistoryDataAcquisition
{
    /// <summary>
    /// Populates an initial list of history provinces.
    /// </summary>
    public static void PopulateHistoryProvinces(DatabaseProvinces database, string directory)
    {
        string[] historyFiles = Directory.GetFiles(Path.Combine(directory, "history", "provinces"),
            "*.txt", SearchOption.AllDirectories);

        foreach (string file in historyFiles)
        {
            var fileName = Path.GetFileName(file).Replace(".txt", "");
            string[] splitName = fileName.Split(!file.Contains('-') ? ' ' : '-');
            if (uint.TryParse(splitName[0], out var provinceId))
            {
                string provinceName = splitName[1];
                provinceName = splitName.Length == 1 ? splitName[0].Trim() : provinceName.Trim();

                database.SetHistoryData(provinceId, file, provinceName);
            }
            else Debug.WriteLine("Error listing history provinces. File {0} could not be split.", fileName);
        }
    }

    /*
     * TODO: READ IDEOLOGIES FOR IDEOLOGICAL FLAG COMPARISONS
     */
    public static void PopulateHistoryCountries(DatabaseCountries database, string directory)
    {
        // Access all the country history files linked from earlier. Not searching over again for performance and
        //  consistency.
        foreach (var file in database.HistoryFile)
        {
            string[] splitFile = file.Split('\\');
            string tag = splitFile[^1][..3];

            // Database fill-up for the country is done inside the parser.
            new CountryParserHistory(database, directory, tag).Parse<object>();
        }
    }
}