using CsvHelper;
using CsvHelper.Configuration;
using Paradox_Editor.A_Map_Navigation;
using Paradox_Editor.B_Map_Functions;
using Paradox_Editor.C_Window_Functions;
using Paradox_Editor.D_Static_Variables;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Color = System.Windows.Media.Color;

namespace Paradox_Editor
{
    public class MapEditor
    {
        public MapEditor()
        {
        }

        public static void UpdatePoliticalMap() //The test process for how map loading should work.
        {
            var MainWindow = (MainWindow)Application.Current.MainWindow;

            //string TestCountriesTextFile = new(Properties.Resources.TestCountries);


            /*This is a test origin. Later will be set by fileselect.*/
            var origin = "C:\\Users\\LukeZurg22_Gaming\\source\\repos\\Paradox Editor\\TestEnvironmentFolder\\";
            /*This is a test "CSV" file. Later will use the fileselect's current found CSV.*/
            var countriestxtlines = File.ReadAllLines("C:\\Users\\LukeZurg22_Gaming\\source\\repos\\Paradox Editor\\TestEnvironmentFolder\\TestCountries.txt");

            var colorToProvinceId = new Dictionary<string, string>();
            var cfg = new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = ";" };
            using (var reader = new StreamReader(@"C:\Users\LukeZurg22_Gaming\source\repos\Paradox Editor\TestEnvironmentFolder\testdefinition.csv"))
            using (var csv = new CsvReader(reader, cfg))
            {
                colorToProvinceId = csv.GetRecords<ProvinceDefinition>().ToDictionary(c => c.red + " " + c.green + " " + c.blue, c => c.province);
            }

            var provinceIDToFile = new Dictionary<string, string>();
            var provinceIDToProvinceName = new Dictionary<string, string>();
            var provinceIDToHistoryFile = new Dictionary<string, HistoryFile>();
            var provinceIDToTAG_Core = new Dictionary<string, string>(); //Directly owned by a country
            var provinceIDToTAG_Controller = new Dictionary<string, string>(); //Stripey lines; controlled by country

            var FolderSources = new FolderSelect();
            FolderSources.CollectDirectoryData("CollectDirectory");
            var vic2Provinces = FolderSources.FilePath1;

            var Text = new TextFileExtract(vic2Provinces, provinceIDToFile, provinceIDToProvinceName, provinceIDToHistoryFile);
            Text.ExtractForDictionary("Dictionary");
            provinceIDToFile = Text.provinceIDToFileDictionary; //Adds Province ID's & full paths to said-file in a dictionary
            provinceIDToProvinceName = Text.provinceIDToProvinceNameDictionary;

            provinceIDToHistoryFile = Text.provinceIDToHistoryFileDictionary;

            foreach (KeyValuePair<string, HistoryFile> entry in provinceIDToHistoryFile) //split into new class or into textfilextract
            {
                if (entry.Value.Controller.Count != 0)
                    provinceIDToTAG_Controller.Add(entry.Key, entry.Value.Controller[0]);
                else
                    provinceIDToTAG_Controller.Add(entry.Key, "noController");
            }

            //provinceIDToTAG_Controller = 
            //Text.Dictionary4;

            var tagToCountry = new Dictionary<string, string>();
            foreach (var line in countriestxtlines)
            {
                var input = line;
                var index = input.IndexOf("#");
                if (index >= 0)
                    input = input.Substring(0, index);
                index = input.IndexOf("dynamic_tags");
                if (index >= 0)
                    input = input.Substring(0, index);

                if (string.IsNullOrEmpty(input))
                    continue;

                var removal = input.Replace("\t", "").Replace("\"countries/", "").Replace(".txt\"", "");
                var words = removal.Split('=');
                var trimmedTagToCountry = new string[] { words[0].Trim(), words[1].Trim() };
                Debug.WriteLine(trimmedTagToCountry);
                tagToCountry.Add(trimmedTagToCountry[0], trimmedTagToCountry[1]);

            }

            //

            var provinceIDToCountry = new Dictionary<string, string>()
            {
                { "1", MainWindow.TestPTB1.Text },
                { "2", MainWindow.TestPTB2.Text },
                { "3", MainWindow.TestPTB3.Text },
                { "4", MainWindow.TestPTB4.Text },
                { "5", MainWindow.TestPTB5.Text },
                { "6", MainWindow.TestPTB6.Text },
            };

            var countryToColor = new Dictionary<string, Color>()
            {
                { "Jan Mayen", Color.FromRgb(15, 100, 132)},
                { "Mann", Color.FromRgb(178, 34, 34) },
                { "Pskov", Color.FromRgb(240, 230, 140) }
            };








            //

            var TAGToFile = new Dictionary<string, string>(); //Path to the file i want to open if the country/province is clicked

            var firstLayer = BitmapFactory.ConvertToPbgra32Format((BitmapSource)MainWindow.testimage1.Source);
            var writeableBmp = BitmapFactory.New((int)firstLayer.Width, (int)firstLayer.Height);
            writeableBmp.Clear(Colors.White);

            var ColorMap = new DrawMapColors(firstLayer, writeableBmp, MainWindow.mapBackground, colorToProvinceId, provinceIDToCountry, countryToColor);
            ColorMap.Drawing();
            MainWindow.testimage2.Source = ColorMap.SizeReference;


        }

    }
}