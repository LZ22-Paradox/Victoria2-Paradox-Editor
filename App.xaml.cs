using System.IO;
using System.Windows;

namespace Paradox_Editor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
    }

    public static class ProgramProperties
    {
        public static string ProvinceDirectory { get; set; }

        public class ProvinceFile
        {
            public int ID { get; set; }
            public string ProvinceName { get; set; }
            public string FilePath { get; set; }
        }

        public class Country
        {
            public int ID { get; set; }
            public string CommonFilepath { get; set; }
            public string HistoryFilepath { get; set; }
            public string RGB { get; set; }
        }

        public static string StoredOpener { get; set; } = Path.Combine("E:", "Games", "Victoria II", "mod", "LZ22", "history", "provinces");
        //Stored opener will be subject to change for user convienence
    }

}
