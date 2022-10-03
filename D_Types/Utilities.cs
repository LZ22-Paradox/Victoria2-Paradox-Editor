using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

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

    public struct HistoryFile
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

    public struct ProvinceCSVDefinition
    {
        public string province { get; set; }
        public string red { get; set; }
        public string green { get; set; }
        public string blue { get; set; }
        public string name { get; set; }
    }

    public struct DirectoryStructure
    {
        public string[] MasterDirectory { get; set; }
        public string[] Countries { get; set; }
        public string[] HistoryProvincePaths { get; set; }
        public string CountriesTxt { get; set; }
        public string DefinitionCSVPath { get; set; }

    }

    [Obsolete] //Code below may be obsolete and will need revision.
    public static class BitmapFromSource
    {
        public static Bitmap BmpFromSource(BitmapSource bitmapsource)
        {
            Bitmap bitmap;
            using (var outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapsource));
                enc.Save(outStream);
                bitmap = new Bitmap(outStream);
            }
            return bitmap;
        }
    }

}