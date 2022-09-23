using System.Collections.Generic;
using System.Diagnostics;

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

        //History File
        public string Owner { get; set; }
        public string Controller { get; set; }
        public List<string> Cores;
        public string TradeGoods { get; set; }
        public int LifeRating { get; set; }
        public string Terrain { get; set; }
        public int Colonial { get; set; }
        public List<StateBuilding> State_Buildings { get; set; }
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
                    this.ProvinceName = record.name;

                    // Debug.WriteLine("(RGB: {0}, ID: {1}, NAME: {2})", color, record.province, record.name);
                }
            }
        }

        public void PopulateStateBuildings()
        {

        }

        public void PopulateCoreList()
        {

        }
    }
}
