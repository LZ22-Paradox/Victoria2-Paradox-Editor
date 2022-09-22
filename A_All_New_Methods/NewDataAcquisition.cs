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
using static Paradox_Editor.FileDataAcquisition;

namespace Paradox_Editor.A_All_New_Methods
{

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

    public class NewDataAcquisition
    {
        private DirectoryStructure Directories = new();
        private Dictionary<uint, ProvinceFile> Provinces = new();

        //Temporary lists that may be sorted later
        private Dictionary<uint, int> ColorsToProvinceIDs = new();
        private Dictionary<string, string> TagsToCountryNames = new();


        public NewDataAcquisition() { }
        public NewDataAcquisition(string directory)
        {
            Directories = CollectDirectoryData(directory);
        }
        public Dictionary<uint, ProvinceFile> GetProvinces() => Provinces;

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
        public void ListHistoryprovinces()
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
                    tempProvinceFile.ProvinceFileName = splitName[1];
                    if (Provinces.ContainsKey((uint)IDValue))
                        Debug.WriteLine("Repeated Entry | " + IDValue);
                    else
                        Provinces.Add((uint)IDValue, tempProvinceFile); //provinceIDToFile
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
                    try { Provinces[Convert.ToUInt32(record.province)].AppendFromCSV(record); }
                    catch (KeyNotFoundException exception) { Debug.WriteLine(exception.Message); }
                }
            }
        }

        #region This region is for code that must be compressed, method-wise, into either the province file for otherwise. (Find more efficient ways to do this!)
        public void GetProvinceColorToID()
        {
            foreach (ProvinceFile province in Provinces.Values)
            {
                ColorsToProvinceIDs.Add(province.color, province.ProvinceID);
            }
        }

        public void GetTagToCountryName() //Requires fixing up and efficiency-working (also implement into provinces)
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

        //Separate State Buildings & Cores into seperate, smaller methods.
        public void ExtractHistoryFileContents()
        {
            foreach (ProvinceFile file in Provinces.Values)
            {
                bool isReadingBuilding = false;
                var stateBuildingTempList = new StateBuilding();
                foreach (var line in File.ReadAllLines(file.HistoryFilePath)) //IMPLIMENT IGNORE LINES WITH A POUND "4" | Delete everything AFTER
                    if (NullOrWhiteSpaceCheck.IsNotEmptyOrWhiteSpace(line.Trim()))
                    {
                        var badLines = new[] { "\t", "#" }; //Hashtag|Pound added to Badlines temporarily.
                        var seperatedLines = line.Replace(" ", "").Split('='); //Ignoring state-buildings. Do that later!
                        var key = seperatedLines[0];
                        var value = "";
                        if (!line.Contains("}"))
                            value = seperatedLines[1]; //Ignore "}" lines
                        else
                            value = "}";

                        if (line.Contains("state_building = {", StringComparison.Ordinal))
                        {
                            isReadingBuilding = true;
                            stateBuildingTempList = new StateBuilding();
                        }
                        else if (line.Contains("level", StringComparison.Ordinal))
                        {
                            stateBuildingTempList.Level = value;
                        }
                        else if (line.Contains("building", StringComparison.Ordinal))
                        {
                            stateBuildingTempList.Building = value;
                        }
                        else if (line.Contains("upgrade", StringComparison.Ordinal))
                        {
                            stateBuildingTempList.Upgrade = value;
                        }
                        else if (line.Contains("}") && (isReadingBuilding == true))
                        {
                            isReadingBuilding = false;
                            file.State_Buildings.Add(stateBuildingTempList);
                        }
                        if (!isReadingBuilding)
                        {
                            if (badLines.Any(line.Contains).Equals(false))
                            {

                                if (key.Equals("owner", StringComparison.Ordinal))
                                    file.Owner = (value);

                                else if (key.Equals("controller", StringComparison.Ordinal))
                                    file.Controller = (value);

                                else if (key.Equals("trade_goods", StringComparison.Ordinal))
                                    file.TradeGoods = (value);

                                else if (key.Equals("life_rating", StringComparison.Ordinal))
                                    file.LifeRating = Convert.ToInt16(value);

                                else if (key.Equals("colonial", StringComparison.Ordinal))
                                    file.Colonial = Convert.ToInt16(value);

                                else if (key.Equals("terrain", StringComparison.Ordinal))
                                    file.Terrain = (value);

                                else if (key.Equals("add_core", StringComparison.Ordinal))
                                    file.Cores = (value); //Sent to sub-method in provincefile, see better algorithms

                                else if (key.Equals("naval_base", StringComparison.Ordinal))
                                    file.Naval_Base = Convert.ToInt16(value);

                                else if (key.Equals("fort", StringComparison.Ordinal))
                                    file.Fort = Convert.ToInt16(value);

                                else if (key.Equals("railroad", StringComparison.Ordinal))
                                    file.Railroad = Convert.ToInt16(value);
                            }
                        }
                    }
            }
        }

    }

    #endregion
}
