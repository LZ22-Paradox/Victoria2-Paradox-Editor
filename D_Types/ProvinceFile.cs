using System.Collections.Generic;

namespace Paradox_Editor.D_Types
{
    public class ProvinceFile
    {
        //File IO Data
        public int ProvinceID { get; set; }
        public string ProvinceName { get; set; }
        public string FilePath { get; set; }
        public string ProvinceFileName { get; set; }
        public string red { get; set; }
        public string green { get; set; }
        public string blue { get; set; }
        public string[] RGB { get; set; }

        //History File
        public List<string> Owner { get; set; }
        public List<string> Controller { get; set; }
        public List<string> Core { get; set; }
        public List<string> TradeGoods { get; set; }
        public List<string> LifeRating { get; set; }
        public List<string> Terrain { get; set; }
        public List<string> Colonial { get; set; }
        public List<StateBuilding> State_Building { get; set; }
        public List<string> Naval_Base { get; set; }
        public List<string> Fort { get; set; }
        public List<string> Railroad { get; set; }
    }
}
