using Paradox_Editor.D_Types;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Paradox_Editor.B_Map_Functions
{
    public class DataExtractor
    {

        public string[] FilePath;
        public Dictionary<string, string> provinceIDToFileDictionary;
        public Dictionary<string, string> provinceIDToProvinceNameDictionary;
        public Dictionary<string, HistoryFile> provinceIDToHistoryFileDictionary;
        public Dictionary<string, string> provinceIDToControllerDictionary;

        public class ProvinceIDDictionaries
        {
            public Dictionary<string, string> ToFile { get; set; }
            public Dictionary<string, string> ToName { get; set; }
            public Dictionary<string, HistoryFile> ToHistoryFile { get; set; }
        }

        public ObservableCollection<HistoryFile> HistoryFileData { get; set; } = new ObservableCollection<HistoryFile>();

        public DataExtractor(string[] filepath, ObservableCollection<ProvinceFile> datasend)
        {
            FilePath = filepath;
        }

        public DataExtractor(string[] filepath, Dictionary<string, string> dictionary1, Dictionary<string, string> dictionary2, Dictionary<string, HistoryFile> dictionary3)
        {
            FilePath = filepath;
            provinceIDToFileDictionary = dictionary1; //provinceIDToFile
            provinceIDToProvinceNameDictionary = dictionary2; //provinceIDToProvinceName
            provinceIDToHistoryFileDictionary = dictionary3; //provinceIDToHistoryFile
        }

        public ObservableCollection<ProvinceFile> ExtractForCollection(string[] FilePath) //Repair Line 44
        {
            ObservableCollection<ProvinceFile> ProvinceData = new ObservableCollection<ProvinceFile>();
            foreach (string fileEntry in FilePath) //"For each file in the path list"
            {
                var fileName = Path.GetFileName(fileEntry);
                var SplitName = fileName.Split('-');
                if (int.TryParse(SplitName[0], out int IDValue))
                {
                    ProvinceData.Add(new ProvinceFile() { ProvinceID = IDValue, ProvinceName = SplitName[1], HistoryFilePath = fileEntry }); //Add the respective province data into the FilePaths source
                }
                else
                {
                    Debug.WriteLine("Problem File(s) |" + " Collection Extractor | " + SplitName[0]);
                }
            }
            return ProvinceData;
        }

        public ProvinceIDDictionaries ExtractForDictionary()
        {
            foreach (string fileEntry in FilePath)
            {
                var ownerList = new List<string>();
                var controllerList = new List<string>();
                var coreList = new List<string>();
                var tradeGoodList = new List<string>();
                var lifeRatingList = new List<string>();
                var terrainList = new List<string>();
                var colonialList = new List<string>();

                var navalBaseList = new List<string>(); //Untouched. Add naval base support to interface.
                var fortList = new List<string>();
                var railRoadList = new List<string>();

                var stateBuildingList = new List<StateBuilding>();


                var fileName = Path.GetFileName(fileEntry).Replace(".txt", ""); //FileEntry = Filepath
                var splitName = fileName.Split('-'); //SplitName[1] = Province Name
                if (int.TryParse(splitName[0], out int IDValue)) //IDValue = Province ID
                {
                    if (provinceIDToFileDictionary.ContainsKey(Convert.ToString(IDValue)))
                    {
                        Debug.WriteLine("Repeated Entry | " + IDValue);
                    }
                    else
                    {
                        provinceIDToFileDictionary.Add(Convert.ToString(IDValue), fileEntry); //provinceIDToFile
                    }
                    if (provinceIDToProvinceNameDictionary.ContainsKey(Convert.ToString(IDValue)))
                    {
                        Debug.WriteLine("Repeated Entry | " + IDValue);
                    }
                    else
                    {
                        provinceIDToProvinceNameDictionary.Add(Convert.ToString(IDValue), splitName[1].Replace(" ", ""));
                    }
                }
                else
                {
                    Debug.WriteLine("Problem File(s) | Dictionary Extractor -TextFileExtract.cs");
                }

                bool isReadingBuilding = false;
                StateBuilding stateBuildingTempList = new StateBuilding();
                foreach (var line in File.ReadAllLines(fileEntry))
                //IMPLIMENT IGNORE LINES WITH A POUND "4" | Delete everything AFTER the #. Otherwise, some line of lua may be lost
                {
                    if (NullOrWhiteSpaceCheck.IsNotEmptyOrWhiteSpace(line.Trim()))
                    {
                        var badLines = new[] { "\t", "#" }; //Hashtag|Pound added to Badlines temporarily. Add interactions s o o n :tm:
                        var seperatedLines = line.Replace(" ", "").Split('='); //Ignoring state-buildings. Do that later!
                        var key = seperatedLines[0];
                        var value = "";
                        if (!line.Contains("}"))
                        {
                            value = seperatedLines[1]; //Ignore "}" lines
                        } else
                        {
                            value = "}";
                        }
                        
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
                            stateBuildingList.Add(stateBuildingTempList);
                        }

                        if (!isReadingBuilding)
                        {
                            if (badLines.Any(line.Contains).Equals(false))
                            {

                                if (key.Equals("owner", StringComparison.Ordinal))
                                    ownerList.Add(value);

                                else if (key.Equals("controller", StringComparison.Ordinal))
                                    controllerList.Add(value);

                                else if (key.Equals("trade_goods", StringComparison.Ordinal))
                                    tradeGoodList.Add(value);

                                else if (key.Equals("life_rating", StringComparison.Ordinal))
                                    lifeRatingList.Add(value);

                                else if (key.Equals("colonial", StringComparison.Ordinal))
                                    colonialList.Add(value);

                                else if (key.Equals("terrain", StringComparison.Ordinal))
                                    terrainList.Add(value);

                                else if (key.Equals("add_core", StringComparison.Ordinal))
                                    coreList.Add(value);

                                else if (key.Equals("naval_base", StringComparison.Ordinal))
                                    navalBaseList.Add(value);

                                else if (key.Equals("fort", StringComparison.Ordinal))
                                    fortList.Add(value);

                                else if (key.Equals("railroad", StringComparison.Ordinal))
                                    railRoadList.Add(value);
                            }
                        }
                    }
                }

                var historyFile = new HistoryFile()
                {
                    Owner = ownerList,
                    Controller = controllerList,
                    Core = coreList,
                    TradeGoods = tradeGoodList,
                    LifeRating = lifeRatingList,
                    Terrain = terrainList,
                    State_Building = stateBuildingList,
                    Colonial = colonialList,
                    Naval_Base = navalBaseList,
                    Fort = fortList,
                    Railroad = railRoadList,
                };

                HistoryFileData.Add(historyFile);

                if (!provinceIDToHistoryFileDictionary.ContainsKey(Convert.ToString(IDValue)))
                    provinceIDToHistoryFileDictionary.Add(Convert.ToString(IDValue), historyFile); //duplicates entries. This is not needed.
                else
                    Debug.WriteLine("Repeated Entry | " + IDValue);
                }

            return new ProvinceIDDictionaries()
            {
                ToFile = provinceIDToFileDictionary,
                ToName = provinceIDToProvinceNameDictionary,
                ToHistoryFile = provinceIDToHistoryFileDictionary
            };
        }



    }
}
