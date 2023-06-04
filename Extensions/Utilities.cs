using System.IO;
using System.Linq;

namespace Paradox_Editor.Extensions
{
    public class Building
    {

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

    public static class Utilities
    {
        public static string RemoveWhitespace(this string input)
        {
            return new string(input.ToCharArray()
                .Where(c => !System.Char.IsWhiteSpace(c))
                .ToArray());
        }

        public static bool CheckIfInMod(string directory)
        {
            if (File.Exists(directory))
                return true;
            return false;
        }
    }
}