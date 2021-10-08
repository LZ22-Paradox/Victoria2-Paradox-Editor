using CsvHelper;
using CsvHelper.Configuration;
using Paradox_Editor.A_Map_Navigation;
using Paradox_Editor.B_Map_Functions;
using Paradox_Editor.C_Window_Functions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
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

            string TestCountriesTextFile = new(Properties.Resources.TestCountries);


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
            var provinceIDToTAG_Core = new Dictionary<string, string>(); //Directly owned by a country
            var provinceIDToTAG_Controller = new Dictionary<string, string>(); //Stripey lines; controlled by country

            FolderSelect FolderSources = new FolderSelect();
            FolderSources.CollectDirectoryData("CollectDirectory");
            var vic2Provinces = FolderSources.FilePath1;

            TextFileExtract Text = new TextFileExtract(vic2Provinces, provinceIDToFile, provinceIDToProvinceName, provinceIDToTAG_Core, provinceIDToTAG_Controller);
            Text.ExtractForDictionary("Dictionary");
            provinceIDToFile = Text.Dictionary1; //Adds Province ID's & full paths to said-file in a dictionary
            provinceIDToProvinceName = Text.Dictionary2;
            provinceIDToTAG_Core = Text.Dictionary3;
            provinceIDToTAG_Controller = Text.Dictionary4;


            var tagToCountry = new Dictionary<string, string>(); //You know the drill!
            foreach (var line in countriestxtlines)
            {
                string input = line;
                int index = input.IndexOf("#");
                if (index >= 0)
                    input = input.Substring(0, index);
                index = input.IndexOf("dynamic_tags");
                if (index >= 0)
                    input = input.Substring(0, index);

                if (input != "")
                {
                    string removal = input.Replace("\t", "").Replace("\"countries/", "").Replace(".txt\"", "");
                    string[] words = removal.Split('=');
                    string[] trimmedTagToCountry = { words[0].Trim(), words[1].Trim() };
                    Debug.WriteLine(trimmedTagToCountry);
                   tagToCountry.Add(trimmedTagToCountry[0], trimmedTagToCountry[1]);
                }
            }

            //find way to merge provinceIDToFile to 

            var provinceIDToCountry = new Dictionary<string, string>();
            provinceIDToCountry.Add("1", MainWindow.TestPTB1.Text);
            provinceIDToCountry.Add("2", MainWindow.TestPTB2.Text);
            provinceIDToCountry.Add("3", MainWindow.TestPTB3.Text);
            provinceIDToCountry.Add("4", MainWindow.TestPTB4.Text);
            provinceIDToCountry.Add("5", MainWindow.TestPTB5.Text);
            provinceIDToCountry.Add("6", MainWindow.TestPTB6.Text);

            var countryToColor = new Dictionary<string, Color>();
            countryToColor.Add("Jan Mayen", Color.FromRgb(15, 100, 132));
            countryToColor.Add("Mann", Color.FromRgb(178, 34, 34));
            countryToColor.Add("Pskov", Color.FromRgb(240, 230, 140));





            var TAGToFile = new Dictionary<string, string>(); //Path to the file i want to open if the country/province is clicked




            var firstLayer = BitmapFactory.ConvertToPbgra32Format((BitmapSource)MainWindow.testimage1.Source);
            var writeableBmp = BitmapFactory.New((int)firstLayer.Width, (int)firstLayer.Height);
            writeableBmp.Clear(Colors.White);

            DrawMapColors ColorMap = new DrawMapColors(firstLayer, writeableBmp, MainWindow.mapBackground, colorToProvinceId, provinceIDToCountry, countryToColor);
            ColorMap.Drawing();
            MainWindow.testimage2.Source = ColorMap.SizeReference;


        }

    }
}