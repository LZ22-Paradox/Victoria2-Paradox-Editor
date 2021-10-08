using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Paradox_Editor.ProgramProperties;

namespace Paradox_Editor.B_Map_Functions
{
    public class TextFileExtract
    {
        public string[] FilePath;
        private string[] SeperatedLines;
        public Dictionary<string, string> Dictionary1;
        public Dictionary<string, string> Dictionary2;
        public Dictionary<string, string> Dictionary3;
        public Dictionary<string, string> Dictionary4;

        public ObservableCollection<ProvinceFile> OutgoingData { get; set; }

        public ObservableCollection<HistoryFile> InnerData { get; set; } = new ObservableCollection<HistoryFile>();


        public TextFileExtract(string[] filepath, ObservableCollection<ProvinceFile> datasend)
        {
            FilePath = filepath;
            OutgoingData = datasend;
        }

        public TextFileExtract(string[] filepath, Dictionary<string, string> dictionary1, Dictionary<string, string> dictionary2, Dictionary<string, string> dictionary3, Dictionary<string, string> dictionary4)
        {
            FilePath = filepath;
            Dictionary1 = dictionary1; //provinceIDToFile
            Dictionary2 = dictionary2; //provinceIDToProvinceName
            Dictionary3 = dictionary3; //provinceIDToTAG_Owner
            Dictionary4 = dictionary4; //provinceIDToTAG_Controller
        }

        public bool ExtractForCollection(string Collection)
        {
            foreach (string fileEntry in FilePath) //"For each file in the path list"
            {
                string fileName = Path.GetFileName(fileEntry);
                string[] SplitName = fileName.Split('-');
                if (int.TryParse(SplitName[0], out int IDValue))
                {
                    OutgoingData.Add(new ProvinceFile() { ProvinceID = IDValue, ProvinceName = SplitName[1], FilePath = fileEntry }); //Add the respective province data into the FilePaths source
                }
                else
                {
                    Debug.WriteLine("Problem File(s) |" + " Collection Extractor | " + SplitName[0]);
                }

            }
            return true;
        }


        public bool ExtractForDictionary(string Dictionary)
        {
            var ownerList = new List<string>();
            var controllerList = new List<string>();
            var coreList = new List<string>();
            var tradeGoodList = new List<string>();
            var lifeRatingList = new List<string>();
            var terrainList = new List<string>();
            var colonialList = new List<string>();

            foreach (string fileEntry in FilePath) //"For each file in the path list"
            {
                string fileName = Path.GetFileName(fileEntry); //FileEntry = Filepath
                string[] SplitName = fileName.Split('-'); //SplitName[1] = Province Name
                if (int.TryParse(SplitName[0], out int IDValue)) //IDValue = Province ID
                {
                    if (Dictionary1.ContainsKey(Convert.ToString(IDValue)))
                    {
                        Debug.WriteLine("Repeated Entry | " + IDValue);
                    }
                    else
                    {
                        Dictionary1.Add(Convert.ToString(IDValue), fileEntry); //provinceIDToFile
                    }
                    if (Dictionary2.ContainsKey(Convert.ToString(IDValue)))
                    {
                        Debug.WriteLine("Repeated Entry | " + IDValue);
                    }
                    else
                    {
                        Dictionary2.Add(Convert.ToString(IDValue), SplitName[1].Replace(" ", ""));
                    }
                }
                else
                {
                    Debug.WriteLine("Problem File(s) | Dictionary Extractor");
                }

                foreach (var line in File.ReadAllLines(fileEntry)) //Where the magic happens
                {

                    SeperatedLines = line.Replace(" ", "").Split('=');
                    if (SeperatedLines[0] == "owner")
                    {
                        ownerList.Add(SeperatedLines[1]);
                    }
                    if (SeperatedLines[0] == "controller")
                    {
                        controllerList.Add(SeperatedLines[1]);
                    }
                    if (SeperatedLines[0] == "trade_goods")
                    {
                        tradeGoodList.Add(SeperatedLines[1]);
                    }
                    if (SeperatedLines[0] == "life_rating")
                    {
                        lifeRatingList.Add(SeperatedLines[1]);
                    }
                    if (SeperatedLines[0] == "colonial")
                    {
                        colonialList.Add(SeperatedLines[1]);
                    }
                    if (SeperatedLines[0] == "terrain")
                    {
                        terrainList.Add(SeperatedLines[1]);
                    }
                    if (SeperatedLines[0] == "add_core")
                    {
                        coreList.Add(SeperatedLines[1]);
                    }
                }

                InnerData.Add(new HistoryFile()
                {
                    Controller = controllerList,
                    Core = coreList,
                    TradeGoods = tradeGoodList,
                    LifeRating = lifeRatingList,
                    Terrain = terrainList,
                    Colonial = colonialList
                }); ;
                ownerList.Clear();
                controllerList.Clear();
                coreList.Clear();
                tradeGoodList.Clear();
                lifeRatingList.Clear();
                terrainList.Clear();
                colonialList.Clear();


            }
            return true;
        }

    }
}
