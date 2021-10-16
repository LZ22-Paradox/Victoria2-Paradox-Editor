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

            var colorToProvinceId = new Dictionary<string, string>();
            var cfg = new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = ";" };
            using (var reader = new StreamReader(FolderSelect.Path_CSV[0]))
            using (var csv = new CsvReader(reader, cfg))
            {
                colorToProvinceId = csv.GetRecords<ProvinceDefinition>().ToDictionary(c => c.red + " " + c.green + " " + c.blue, c => c.province);
            }

            var provinceIDToFile = new Dictionary<string, string>();
            var provinceIDToProvinceName = new Dictionary<string, string>();
            var provinceIDToHistoryFile = new Dictionary<string, HistoryFile>();
            var provinceIDToCoreTAG = new Dictionary<string, string>(); //Stripey Green Lines; Core from a country
            var provinceIDToControllerTAG = new Dictionary<string, string>(); //Stripey lines; controlled by country

            var FolderSources = new FolderSelect();
            FolderSources.CollectDirectoryData("CollectDirectory");
            var vic2Provinces = FolderSources.Path_Provinces;

            var Text = new TextFileExtract(vic2Provinces, provinceIDToFile, provinceIDToProvinceName, provinceIDToHistoryFile);
            Text.ExtractForDictionary("Dictionary");
            provinceIDToFile = Text.provinceIDToFileDictionary; //Adds Province ID's & full paths to said-file in a dictionary
            provinceIDToProvinceName = Text.provinceIDToProvinceNameDictionary;

            foreach (KeyValuePair<string, HistoryFile> entry in Text.provinceIDToHistoryFileDictionary) //split into new class or into textfilextract
            {
                if (entry.Value.Controller.Count != 0)
                    provinceIDToControllerTAG.Add(entry.Key, entry.Value.Controller[0]);
                else
                    provinceIDToControllerTAG.Add(entry.Key, "noController");
            }

            var tagToCountryName = new Dictionary<string, string>();
            foreach (var line in File.ReadAllLines(FolderSelect.Path_CountriesTxt[0]))
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
                if (!tagToCountryName.ContainsKey(trimmedTagToCountry[0])) {
                tagToCountryName.Add(trimmedTagToCountry[0], trimmedTagToCountry[1]);
                } else
                {
                    Debug.WriteLine("Repeated TAG in common/countries/: " + trimmedTagToCountry[0]);
                }
            }

            var countryNameToColor = new Dictionary<string, Color>();
            CountryNameToColor CountryNameToColorConverter = new CountryNameToColor(FolderSelect.Path_Countries);
            CountryNameToColorConverter.GetCountryColor();
            countryNameToColor = CountryNameToColorConverter.NameToColor;

            ///I Have:::
            ///countryNameToColor
            ///tagToCountryName
            ///provinceIDToControllerTAG
            ///colorToProvinceId
            
            var firstLayer = BitmapFactory.ConvertToPbgra32Format((BitmapSource)MainWindow.mapBackground.Source); //May be problem
            var writeableBmp = BitmapFactory.New((int)firstLayer.PixelWidth, (int)firstLayer.PixelHeight); //Different dimensions than firstlayer
            writeableBmp.Clear(Colors.White);
            
            //WriteableBitmap firstlayer,
            //WriteableBitmap sizereference,
            //Image image,

            var ColorMap = new MapRenderer(firstLayer, writeableBmp, MainWindow.mapBackground, colorToProvinceId, provinceIDToControllerTAG, tagToCountryName, countryNameToColor);
            ColorMap.Drawing();
            MainWindow.mapPolitical.Source = ColorMap.SizeReference;


        }
    }
}