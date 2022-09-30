using CSVFile;
using CsvHelper;
using CsvHelper.Configuration;
using Paradox_Editor.D_Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Paradox_Editor.A_All_New_Methods
{
    //For use in important CSV file reading.
    public sealed class MainCsvIndexSyntax : ClassMap<ProvinceFile>
    {
        public MainCsvIndexSyntax()
        {
            Map(m => m.ProvinceID).Index(0);
            Map(m => m.red).Index(1);
            Map(m => m.green).Index(2);
            Map(m => m.blue).Index(3);
            Map(m => m.ProvinceName).Index(3);
        }
    }

    public class DataAcquisition
    {
        private DirectoryStructure Directories = new();
        private Dictionary<uint, ProvinceFile> Provinces = new();

        //Temporary lists that may be sorted later
        private Dictionary<uint, int> ColorsToProvinceIDs = new();
        private Dictionary<string, string> TagsToCountryNames = new();
        private Dictionary<string, Color> CountryNamesToColors = new();

        public DataAcquisition() { }
        public DataAcquisition(string directory)
        {
            Directories = CollectDirectoryData(directory);
        }
        public Dictionary<uint, ProvinceFile> GetProvinces() => Provinces;

        /// <summary>
        /// Calls for a read of all data.
        /// </summary>
        /// <param name="directory"></param>
        public void AcquisitionAllData(string directory)
        {
            CollectDirectoryData(directory);
            GetHistoryFiles();
            PopulateAppendProvinceCSVData();

            GetProvinceColorToID();
            GetTagsToCountryNames();
            GetCountryNamesToColours();
        }
        public DataAcquisition ReturnAcquisitionAllData(string directory)
        {
            AcquisitionAllData(directory);
            return this;
        }

        /// <summary>
        /// Gets Victoria 2's directory paths.
        /// </summary>
        /// <param name="directory"></param>
        public DirectoryStructure CollectDirectoryData(string directory)
        {
            string[] masterFolder = Directory.GetFiles(directory, "*.txt", SearchOption.AllDirectories);
            string[] historyProvincePaths = Directory.GetFiles(Path.Combine(directory, "history", "provinces"), "*.txt", SearchOption.AllDirectories);
            string[] countryCommonFiles = Directory.GetFiles(Path.Combine(directory, "common", "countries"), "*.txt", SearchOption.AllDirectories);
            string countriesCommonFilePath = Path.Combine(directory, "common", "countries.txt");
            string CSVFilePath = Path.Combine(directory, "map", "definition.csv");
            return new DirectoryStructure()
            {
                MasterDirectory = masterFolder,
                Countries = countryCommonFiles,
                CountriesTxt = countriesCommonFilePath,
                DefinitionCSVPath = CSVFilePath,
                HistoryProvincePaths = historyProvincePaths
            };
        }

        /// <summary>
        /// Populates an initial list of provinces.
        /// </summary>
        public void GetHistoryFiles()
        {
            ///<!!!!!!DOES NOT READ FILES THAT ONLY HAVE SPACES (IE, LIKE "1337 Prome")!!!!!>
            foreach (string fileEntry in Directories.HistoryProvincePaths)
            {
                var fileName = Path.GetFileName(fileEntry).Replace(".txt", ""); //FileEntry = Filepath
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
                    tempProvinceFile.ProvinceFileName = splitName[1].Trim();
                    if (Provinces.ContainsKey((uint)IDValue))
                        Debug.WriteLine("Repeated Entry | " + IDValue);
                    else
                    {
                        Provinces.Add((uint)IDValue, tempProvinceFile);
                        Provinces[(uint)IDValue].PopulateHistoryData(); //1st Province (Fez) Isn't getting its data!
                    }
                }
                else
                    Debug.WriteLine("Error listing history provinces. File {0} could not be split.", fileName);
                Provinces.OrderBy(x => x.Key);
                //var sorts = from pair in HistoryProvinces orderby pair.Value.ProvinceID ascending select pair;
            }
        }

        /// <summary>
        /// Populates existing provinces with CSV colour & filename data.
        /// </summary>
        /// <param name="pathToCSVFile"></param>
        /// <returns name="Dictionary<uint, string>"></returns>
        public void PopulateAppendProvinceCSVData()
        {
            var cfg = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ";",
                HasHeaderRecord = false,
            };
            using var reader = new StreamReader(Directories.DefinitionCSVPath);
            using var csv = new CsvReader(reader, cfg);
            csv.Context.RegisterClassMap<MainCsvIndexSyntax>();
            var records = csv.GetRecords<ProvinceCSVDefinition>().Skip(1);
            {
                foreach (var record in records)
                {
                    if (record.province.IsNotEmptyOrWhiteSpace())
                    {
                        if (Provinces.ContainsKey(Convert.ToUInt32(record.province)))
                            Provinces[Convert.ToUInt32(record.province)].AppendFromCSV(record);
                    }
                }
            }
        }

        /// <summary>
        /// Populates dictionary of province colour keys with province ID's.
        /// </summary>
        public void GetProvinceColorToID()
        {
            foreach (ProvinceFile province in Provinces.Values)
            {
                ColorsToProvinceIDs.Add(province.color, province.ProvinceID);
            }
        }

        /// <summary>
        /// Gets given game TAG's and country names, based on the common/...name.txt files.
        /// </summary>
        public void GetTagsToCountryNames() //Requires fixing up and efficiency-working (also implement into provinces)
        {
            foreach (var line in File.ReadAllLines(Directories.CountriesTxt)) //Check Path_CountriesTxt
            {
                var input = line;
                var index = input.IndexOf("#");
                if (index >= 0)
                    input = input.Substring(0, index);
                index = input.IndexOf("dynamic_tags");
                if (index >= 0)
                    input = input.Substring(0, index);
                if (string.IsNullOrEmpty(input))
                    continue;

                var removal = input.Replace("\t", "").Replace("\"countries/", "").Replace(".txt\"", "");
                var words = removal.Split('='); //Has the actual country names
                var trimmedTagToCountry = new string[] { words[0].Trim(), words[1].Trim() };
                //Debug.WriteLine(trimmedTagToCountry);
                if (!TagsToCountryNames.ContainsKey(trimmedTagToCountry[0]))
                    TagsToCountryNames.Add(trimmedTagToCountry[0], trimmedTagToCountry[1]);
                else
                    Debug.WriteLine("Repeated TAG in common/countries/: " + trimmedTagToCountry[0]);
            }
        }

        /// <summary>
        /// Populates the dictionary of country name keys with their respective country-colours.
        /// </summary>
        public void GetCountryNamesToColours()
        {
            foreach (var countryFile in Directories.Countries)
            {
                string[] seperatedColors = new string[3];
                string name = Path.GetFileName(countryFile).Replace(".txt", "");
                string firstLine = File.ReadLines(countryFile).First();
                if (firstLine.StartsWith("color"))
                {
                    var splitData = firstLine.Split("=", StringSplitOptions.TrimEntries);
                    var fixedColorData = splitData[1].Replace("{", "").Replace("}", "").Trim();
                    var splitColors = fixedColorData.Split(" ");
                    seperatedColors = splitColors.Where(x => !string.IsNullOrEmpty(x)).ToArray();
                }
                else
                {
                    foreach (var line in File.ReadAllLines(countryFile)) //Where the magic happens
                    {
                        if (line.Contains("color =", StringComparison.Ordinal)) //color = { #  #  # }
                        {
                            var splitData = line.Split("=", StringSplitOptions.TrimEntries);
                            var fixedColorData = splitData[1].Replace("{", "").Replace("}", "").Trim();
                            var splitColors = fixedColorData.Split(" ");
                            seperatedColors = splitColors.Where(x => !string.IsNullOrEmpty(x)).ToArray();
                        }
                        else { }
                    }
                }
                if (!CountryNamesToColors.ContainsKey(name))
                {
                    CountryNamesToColors.Add(name,
                        Color.FromRgb(
                            (byte)Convert.ToInt32(seperatedColors[0]),
                            (byte)Convert.ToInt32(seperatedColors[1]),
                            (byte)Convert.ToInt32(seperatedColors[2])
                        ));
                }
            }
        }

        //

    }

}
