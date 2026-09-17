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
    public static void PopulateHistoryProvinces(DatabaseProvinces databaseProvinces, string directory)
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

                databaseProvinces.SetFileData(provinceId, file, provinceName);
            }
            else Debug.WriteLine("Error listing history provinces. File {0} could not be split.", fileName);
        }
    }

    /*
     * TODO: ADD READING FOR COUNTRY HISTORY DATA
     * TODO: READ IDEOLOGIES FOR IDEOLOGICAL FLAG COMPARISONS
     */
    public static void PopulateHistoryCountries(DatabaseCountries database, string directory)
    {
        Dictionary<Tag, string> countries = new();
        string folder = Path.Combine(directory, "history", "countries");
        var countryFiles = Directory.GetFiles(folder, "*.txt", SearchOption.AllDirectories);
        foreach (string file in countryFiles)
        {
            string[] splitFile = file.Split('\\');
            string tag = splitFile[^1][..3];
            if (countries.TryAdd(tag, file))
                continue;

            string[] lines = File.ReadAllLines(file);
            MessageBox.Show(@"Duplicate Tag in History Files: " + tag);
            if (lines.Length == 0)
                continue;

            countries.TryGetValue(tag, out var alreadyInsertedCountry);
            if (alreadyInsertedCountry != null)
            {
                lines = File.ReadAllLines(alreadyInsertedCountry);
                if (lines.Length != 0)
                    continue;
            }

            countries.Remove(tag);
            countries.Add(tag, file);
        }


    }
}