using CsvHelper;
using CsvHelper.Configuration;
using Paradox_Editor.Extensions;
using Paradox_Editor.Extensions.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Paradox_Editor.Data_Handling;

public class ProvinceDataAcquisition
{
    private readonly Dictionary<uint, ProvinceFile> Provinces = new();
    private readonly Dictionary<uint, int> ColorsToProvinceIDs = new();
    //private Dictionary<uint, PopulationFile> Populations = new();


    /// <summary>
    /// Derives directory data extraction from the DataAcquisition parent class.
    /// </summary>
    /// <param name="directory"></param>
    public ProvinceDataAcquisition(string directory)
    {
        Task task = Task.Run(() => PopulateProvinceHistoryFiles(directory));
        task.Wait();
        task = Task.Run(PopulateAppendProvinceCSVData);
        task.Wait();
        task = Task.Run(PopulateProvinceColorsToIDs);
        task.Wait();
        //PopulateProvinceHistoryFiles(directory);
        //PopulateAppendProvinceCSVData();
        //PopulateProvinceColorsToIDs();
    }


    public Dictionary<uint, ProvinceFile> GetProvinces() => Provinces;
    public Dictionary<uint, int> GetColorsToProvinceIDs() => ColorsToProvinceIDs;

    public ProvinceFile GetProvince(uint provinceID)
    {
        _ = Provinces.TryGetValue(provinceID, out ProvinceFile province);
        return province;
    }

    public void ReplaceProvince(ProvinceFile province) => Provinces[(uint)province.ProvinceID] = province;

    /// <summary>
    /// Populates an initial list of history provinces.
    /// </summary>
    public void PopulateProvinceHistoryFiles(string directory)
    {
        string[] historyProvincePaths =
            Directory.GetFiles(Path.Combine(directory, "history", "provinces"), "*.txt", SearchOption.AllDirectories);

        foreach (string fileEntry in historyProvincePaths)
        {
            var fileName = Path.GetFileName(fileEntry).Replace(".txt", "");
            string[] splitName = null;
            if (!fileEntry.Contains("-"))
                splitName = fileName.Split(' ');
            else
                splitName = fileName.Split('-'); //SplitName[1] = Province Name
            if (int.TryParse(splitName[0], out int IDValue)) //IDValue = Province ID
            {
                var tempProvinceFile = new ProvinceFile();
                tempProvinceFile.ProvinceID = IDValue;
                tempProvinceFile.HistoryFilePath = fileEntry;

                if (splitName.Length == 1) //Fix for in case name is only a number
                {
                    tempProvinceFile.ProvinceFileName = splitName[0].Trim();
                }
                else
                {
                    tempProvinceFile.ProvinceFileName = splitName[1].Trim();
                }

                if (!Provinces.ContainsKey((uint)IDValue))
                {
                    Provinces.Add((uint)IDValue, tempProvinceFile.PopulateHistoryData());
                }
            }
            else
                Debug.WriteLine("Error listing history provinces. File {0} could not be split.", fileName);

            _ = Provinces.OrderBy(x => x.Key);
        }
    }

    #region CSV Data Handling

    /// <summary>
    /// For use in important CSV file reading.
    /// </summary>
    public sealed class MainCsvIndexSyntax : ClassMap<ProvinceFile>
    {
        public MainCsvIndexSyntax()
        {
            Map(m => m.ProvinceID).Index(0);
            Map(m => (m.Color >> 16) & 0xFF).Index(1);
            Map(m => (m.Color >> 8) & 0xFF).Index(2);
            Map(m => m.Color & 0xFF).Index(3);
            Map(m => m.ProvinceName).Index(4);
        }
    }

    /// <summary>
    /// Populates existing provinces with CSV colour and filename data.
    /// </summary>
    public void PopulateAppendProvinceCSVData()
    {
        var cfg = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = ";",
            HasHeaderRecord = false,
            MissingFieldFound = null
        };
        using var reader = new StreamReader(ModData.MOD_DATA.GetModDirectories().DefinitionCSVPath);
        using var csv = new CsvReader(reader, cfg);
        csv.Context.RegisterClassMap<MainCsvIndexSyntax>();
        var records = csv.GetRecords<ProvinceCSVDefinition>();
        string regexFilter = @"/province/i";
        Regex rg = new Regex(regexFilter);
        Parallel.ForEach(records, record =>
        {
            if (!string.IsNullOrWhiteSpace(record.province) && !Regex.IsMatch(record.province, @"\bprovince\b"))
            {
                if (Provinces.ContainsKey(Convert.ToUInt32(record.province)))
                    Provinces[Convert.ToUInt32(record.province)].AppendFromCSV(record);
            }
        });

        csv.Dispose();
    }

    #endregion

    /// <summary>
    /// Populates dictionary of province colour keys with province ID's.
    /// </summary>
    public void PopulateProvinceColorsToIDs()
    {
        foreach (ProvinceFile province in Provinces.Values)
        {
            if (!ColorsToProvinceIDs.ContainsKey(province.Color))
                ColorsToProvinceIDs.Add(province.Color, province.ProvinceID);
        }
    }
}