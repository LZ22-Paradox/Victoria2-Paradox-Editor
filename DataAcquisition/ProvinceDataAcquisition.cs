using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using Paradox_Editor.Extensions;
using Paradox_Editor.Parsers;
using Paradox_Editor.Types;

namespace Paradox_Editor.DataAcquisition;

public class ProvinceDataAcquisition
{
    public static ProvinceDatabase CreateDatabase(DefaultMap defaultMapFile, string directory)
    {
        var database = new ProvinceDatabase(defaultMapFile.MaxProvinces);

        // Use the newly-created database and populate it.
        var acquisition = new ProvinceDataAcquisition(database, directory);

        return database;
    }

    /// <summary>
    /// Derives directory data extraction from the DataAcquisition parent class.
    /// </summary>
    /// <param name="database"></param>
    /// <param name="directory"></param>
    public ProvinceDataAcquisition(ProvinceDatabase database, string directory)
    {
        Task task = Task.Run(() => PopulateProvinceHistoryFiles(directory));
        task.Wait();
        task = Task.Run(() =>
        {
            var records = new ProvincesDefinitionsFileParser()
                .Parse<List<ProvinceCSVDefinition>>(directory, "map", "definition.csv");

            Parallel.ForEach(records, record =>
            {
                // Avoid parsing null / blank provinces, and especially entries that begin with entries that aren't
                //  numerical.
                if (string.IsNullOrWhiteSpace(record.Province) ||
                    Regex.IsMatch(record.Province, @"\bprovince\b") ||
                    !int.TryParse(record.Province, out int _))
                    return;

                if (!uint.TryParse(record.Red.Replace(".", ""), out var red) ||
                    !uint.TryParse(record.Green.Replace(".", ""), out var green) ||
                    !uint.TryParse(record.Blue.Replace(".", ""), out var blue))
                    return;

                database.SetCSVData(
                    id: Convert.ToUInt32(record.Province),
                    /*Color*/ red, green, blue,
                    /*Province Name*/ record.Name);
            });
        });
        task.Wait();
        //PopulateProvinceHistoryFiles(directory);
        //PopulateAppendProvinceCSVData();
        //PopulateProvinceColorsToIDs();
    }

    /// <summary>
    /// Populates an initial list of history provinces.
    /// </summary>
    public void PopulateProvinceHistoryFiles(string directory)
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

                ProvinceDatabase.Instance.SetFileData(provinceId, file, provinceName);
            }
            else Debug.WriteLine("Error listing history provinces. File {0} could not be split.", fileName);
        }
    }
}