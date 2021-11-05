using System.Collections.Generic;

namespace Paradox_Editor.D_Class_Types
{
    public class HistoryFile
    {
        public List<string> Owner { get; set; }
        public List<string> Controller { get; set; }
        public List<string> Core { get; set; }
        public List<string> TradeGoods { get; set; }
        public List<string> LifeRating { get; set; }
        public List<string> Terrain { get; set; }
        public List<string> Colonial { get; set; }
        public List<string> State_Building { get; set; } //Currently unused. Will be implimented later. [contains level, building, upgrade]
        public List<string> Naval_Base { get; set; }

    }
}
