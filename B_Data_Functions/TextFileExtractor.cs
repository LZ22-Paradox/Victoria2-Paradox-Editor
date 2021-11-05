using Paradox_Editor.D_Class_Types;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Paradox_Editor.B_Map_Functions
{
    public class TextFileExtractor
    {

        public string[] FilePath;
        public Dictionary<string, string> provinceIDToFileDictionary;
        public Dictionary<string, string> provinceIDToProvinceNameDictionary;
        public Dictionary<string, HistoryFile> provinceIDToHistoryFileDictionary;
        public Dictionary<string, string> provinceIDToControllerDictionary;

        public ObservableCollection<HistoryFile> historyFileData { get; set; } = new ObservableCollection<HistoryFile>();

        public TextFileExtractor(string[] filepath, ObservableCollection<ProvinceFile> datasend)
        {
            FilePath = filepath;
        }

        public TextFileExtractor(string[] filepath, Dictionary<string, string> dictionary1, Dictionary<string, string> dictionary2, Dictionary<string, HistoryFile> dictionary3)
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
                    ProvinceData.Add(new ProvinceFile() { ProvinceID = IDValue, ProvinceName = SplitName[1], FilePath = fileEntry }); //Add the respective province data into the FilePaths source
                }
                else
                {
                    Debug.WriteLine("Problem File(s) |" + " Collection Extractor | " + SplitName[0]);
                }

            }
            return ProvinceData;
        } //Repair Lin4 44

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

                var stateBuildingList = new List<string>(); //Unused. See HistoryFile.cs & Todo.txt
                var navalBaseList = new List<string>();


                var fileName = Path.GetFileName(fileEntry); //FileEntry = Filepath
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

                foreach (var line in File.ReadAllLines(fileEntry))
                //IMPLIMENT IGNORE LINES WITH A POUND "4" | Delete everything AFTER the #. Otherwise, some line of lua may be lost
                {
                    if (NullOrWhiteSpaceCheck.IsNotEmptyOrWhiteSpace(line.Trim()))
                    {
                        if (line.Length is not <= 1)
                        {
                            var badLines = new[] { "}", "upgrade", "building", "level", "state_building", "\t", "#" };
                            //Hashtag|Pound added to Badlines temporarily. Add interactions s o o n :tm:
                            if (badLines.Any(line.Contains) == false)
                            {
                                var seperatedLines = line.Replace(" ", "").Split('='); //Ignoring state-buildings. Do that later!
                                var key = seperatedLines[0];
                                var value = seperatedLines[1];

                                if (line.Contains("owner"))
                                    ownerList.Add(value);

                                else if (line.Contains("controller"))
                                    controllerList.Add(value);

                                else if (line.Contains("trade_goods"))
                                    tradeGoodList.Add(value);

                                else if (line.Contains("life_rating"))
                                    lifeRatingList.Add(value);

                                else if (line.Contains("colonial"))
                                    colonialList.Add(value);

                                else if (line.Contains("terrain"))
                                    terrainList.Add(value);

                                else if (line.Contains("add_core"))
                                    coreList.Add(value);

                                else if (line.Contains("naval_base"))
                                    navalBaseList.Add(value);

                                //add something regarding state_buildings.
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
                    Colonial = colonialList,
                    Naval_Base = navalBaseList
                };

                historyFileData.Add(historyFile);

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
