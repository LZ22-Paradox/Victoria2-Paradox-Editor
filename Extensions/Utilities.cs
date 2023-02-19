using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Paradox_Editor.Extensions
{
    public class Core
    {
        private string tag { get; set; }

        public Core(string TAG)
        {
            this.TAG = TAG;
        }

        public string TAG
        {
            get => tag;
            set => tag = value;
        }
    }

    public class Building {

    }

    public class StateBuilding : Building
    {
        public StateBuilding()
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

    public struct ProvinceCSVDefinition
    {
        public string province { get; set; }
        public string red { get; set; }
        public string green { get; set; }
        public string blue { get; set; }
        public string name { get; set; }
        public string color { get; set; }

    }

    public struct DirectoryStructure
    {
        public string[] PrimaryDirectories { get; set; }
        public string[] HistoryProvincePaths { get; set; }
        public string DefinitionCSVPath { get; set; }
    }

    public class Pop
    {
        public string culture { get; set; }
        public string religion { get; set; }
        public string size { get; set; }
    }

    public class Goods //May be useless; see about goods editing
    {

    }

}