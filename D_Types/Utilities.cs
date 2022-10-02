using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paradox_Editor.D_Types
{
    public class Core
    {
        private string TAG { get; set; }

        public Core(string TAG)
        {
            tag = TAG;
        }

        public string tag
        {
            get => TAG;
            set => TAG = value;
        }

        /*public string TAG
        {
            get { return tag; }
            set { tag = value; }
        }*/

    }

    public class HistoryFile
    {
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

    public class StateBuilding
    {
        public StateBuilding()
        {
            Level = level;
            Building = building;
            Upgrade = upgrade;
        }

        public StateBuilding(string tag)
        {
            Level = level;
            Building = building;
            Upgrade = upgrade;
        }

        private string level { get; set; }
        private string upgrade { get; set; }
        private string building { get; set; }

        public string Level
        {
            get => level;
            set => level = value;
        }
        public string Upgrade
        {
            get => upgrade;
            set => upgrade = value;
        }
        public string Building
        {
            get => building;
            set => building = value;
        }
    }

}