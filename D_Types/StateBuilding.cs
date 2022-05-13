using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paradox_Editor.D_Types
{
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