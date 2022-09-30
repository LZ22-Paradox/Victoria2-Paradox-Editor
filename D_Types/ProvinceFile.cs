using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Paradox_Editor.D_Types
{
    public class ProvinceFile
    {
        //File IO Data
        public int ProvinceID { get; set; } //✓
        public string ProvinceName { get; set; }
        public string HistoryFilePath { get; set; } //✓
        public string ProvinceFileName { get; set; } //✓

        //IO Needed for CSV Processing (Especially do not touch)
        public string red { get; set; }
        public string green { get; set; }
        public string blue { get; set; }
        public uint color { get; set; }

        //public HistoryFile HistoryData { get; set; }
        //History Data
        public string Owner { get; set; }
        public string Controller { get; set; }
        public List<string> Cores = new();
        public string TradeGoods { get; set; }
        public int LifeRating { get; set; }
        public string Terrain { get; set; }
        public int Colonial { get; set; }
        public List<StateBuilding> State_Buildings = new List<StateBuilding>();
        public int Naval_Base { get; set; }
        public int Fort { get; set; }
        public int Railroad { get; set; }

        public void AppendFromCSV(ProvinceCSVDefinition record)
        {
            if (record.province.IsNotEmptyOrWhiteSpace())
            { //Faulty. Some provinces bug. (Tested Vanilla Vic2)
                if (uint.TryParse(record.red, out var red) &&
                    uint.TryParse(record.green, out var green) &&
                    uint.TryParse(record.blue, out var blue))
                {
                    var color = (0xFFu << 24) | ((red & 0xFF) << 16) | ((green & 0xFF) << 8) | (blue & 0xFF);
                    this.color = color;
                    ProvinceName = record.name;

                    // Debug.WriteLine("(RGB: {0}, ID: {1}, NAME: {2})", color, record.province, record.name);
                }
            }
        }

        public void PopulateHistoryData()
        {
            bool isReadingBuildings = false;
            StateBuilding tempStateBuilding = new StateBuilding();
            List<string> tempCoresList = new();
            using StreamReader sr = new(HistoryFilePath);
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                string value;
                string key;
                if (line.Replace("\t", "").Contains("}") || line.IsEmptyOrWhiteSpace())
                    value = null;
                else
                {
                    int index = line.IndexOf("#");
                    if (index >= 0)
                        line = line.Substring(0, index);

                    var seperatedLines = line.Replace(" ", "").Split('=');
                    key = seperatedLines[0];
                    value = seperatedLines[1];
                }

                if (isReadingBuildings == false)
                {
                    switch (line)
                    {
                        case string when line.StartsWith("state_building"): //Problem: It is ignoring state-buildings!
                            isReadingBuildings = true;
                            tempStateBuilding = new();
                            break;
                        case string when line.StartsWith("owner"):
                            Owner = value;
                            break;
                        case string when line.StartsWith("controller"):
                            Controller = value;
                            break;
                        case string when line.StartsWith("trade_goods"):
                            TradeGoods = value;
                            break;
                        case string when line.StartsWith("life_rating"):
                            LifeRating = Convert.ToInt16(value);
                            break;
                        case string when line.StartsWith("colonial"):
                            Colonial = Convert.ToInt16(value);
                            break;
                        case string when line.StartsWith("terrain"):
                            Terrain = value;
                            break;
                        case string when line.StartsWith("add_core"):
                            tempCoresList.Add(value);
                            break;
                        case string when line.StartsWith("naval_base"):
                            Naval_Base = Convert.ToInt16(value);
                            break;
                        case string when line.StartsWith("fort"):
                            Fort = Convert.ToInt16(value);
                            break;
                        case string when line.StartsWith("railroad"):
                            Railroad = Convert.ToInt16(value);
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    switch (line)
                    {
                        case string when line.Contains("level"):
                            tempStateBuilding.Level = value;
                            break;
                        case string when line.Contains("building"):
                            tempStateBuilding.Building = value;
                            break;
                        case string when line.Contains("upgrade"):
                            tempStateBuilding.Upgrade = value;
                            break;
                        default:
                            State_Buildings.Add(tempStateBuilding);
                            isReadingBuildings = false;
                            break;
                    }
                }
            }
            Cores = tempCoresList;
        }

    }
}
