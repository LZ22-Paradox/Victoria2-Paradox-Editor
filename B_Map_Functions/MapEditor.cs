using CsvHelper;
using CsvHelper.Configuration;
using Paradox_Editor.A_Map_Navigation;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
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

        public static void TestProvinceUpdate() //The test process for how map loading should work.
        {
            var MainWindow = (MainWindow)Application.Current.MainWindow;

            string TestCountriesTextFile = new(Properties.Resources.TestCountries);
            //@"/TestEnvironmentFolder/TestCountries.txt";

            //string[] readText = (string[])File.ReadLines(TestCountriesTextFile);

            /*This is a test origin. Later will be set by fileselect.*/
            var origin = "C:\\Users\\LukeZurg22_Gaming\\source\\repos\\Paradox Editor\\TestEnvironmentFolder\\";
            /*This is a test "CSV" file. Later will use the fileselect's current found CSV.*/
            var countriestxtlines = File.ReadAllLines("C:\\Users\\LukeZurg22_Gaming\\source\\repos\\Paradox Editor\\TestEnvironmentFolder\\TestCountries.txt");

            List<string[]> CountriesInTxt = new List<string[]>(); //COUNTRIES.TXT FILE
            foreach (var line in countriestxtlines)
            {
                string input = line;
                int index = input.IndexOf("#");
                if (index >= 0)
                    input = input.Substring(0, index);
                index = input.IndexOf("dynamic_tags");
                if (index >= 0)
                    input = input.Substring(0, index);

                Debug.WriteLine(input);

                if (input != "")
                {
                    string firstremoval = input.Replace("\t", "");
                    //string secondremoval = firstremoval.Replace("=", "");
                    string[] words = firstremoval.Split('=');

                    CountriesInTxt.Add(words); //CONTAINS country TAG & History txt file path

                } ///MAKE NEW CLASS TO SPLIT INFORMATION FROM TEXT FILES
            }

            var insertedIDs = new List<string>();
            var insertedRGBs = new List<string[]>();
            var insertedNames = new List<string>();

            var provinceToCountry = new Dictionary<string, string>();
            provinceToCountry.Add("1", MainWindow.TestPTB1.Text);
            provinceToCountry.Add("2", MainWindow.TestPTB2.Text);
            provinceToCountry.Add("3", MainWindow.TestPTB3.Text);
            provinceToCountry.Add("4", MainWindow.TestPTB4.Text);
            provinceToCountry.Add("5", MainWindow.TestPTB5.Text);
            provinceToCountry.Add("6", MainWindow.TestPTB6.Text);

            var colorToProvinceId = new Dictionary<string, string>();
            var countryToColor = new Dictionary<string, Color>();
            countryToColor.Add("Jan Mayen", Color.FromRgb(15, 100, 132));
            countryToColor.Add("Mann", Color.FromRgb(178, 34, 34));
            countryToColor.Add("Pskov", Color.FromRgb(240, 230, 140));


            var countryToTAG = new Dictionary<string, string>(); //You know the drill!

            var TAGToFile = new Dictionary<string, string>(); //Path to the file i want to open if the country/province is clicked

            var provinceIDToFile = new Dictionary<string, string>(); //Path to the file i want to open if the country/province is clicked

            //class that grabs file, reads it, and prepares data
            var cfg = new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = ";" };
            using (var reader = new StreamReader(@"C:\Users\LukeZurg22_Gaming\source\repos\Paradox Editor\TestEnvironmentFolder\testdefinition.csv"))
            using (var csv = new CsvReader(reader, cfg))
            {
                colorToProvinceId = csv.GetRecords<ProvinceDefinition>().ToDictionary(c => c.red + " " + c.green + " " + c.blue, c => c.province);
            }

            var dii = new DrawMapColors(MainWindow.mapBackground, colorToProvinceId);


            /*
                        var firstLayer = BitmapFactory.ConvertToPbgra32Format((BitmapSource)MainWindow.testimage1.Source);
                        var writeableBmp = BitmapFactory.New((int)firstLayer.Width, (int)firstLayer.Height);
                        writeableBmp.Clear(Colors.White);
                        for (int x = 0; x < firstLayer.Width; x++) //REUSE LATER. STATIC CLASS BAD
                        {
                            for (int y = 0; y < firstLayer.Height; y++)
                            {
                                var pixel = firstLayer.GetPixel(x, y);
                                if (
                                    colorToProvinceId.TryGetValue(pixel.R + " " + pixel.G + " " + pixel.B, out var provinceID) &&
                                    provinceToCountry.TryGetValue(provinceID, out var country) && 
                                    countryToColor.TryGetValue(country, out var countryColor))
                                {
                                    writeableBmp.FillRectangle(x, y, x+1, y+1, countryColor); //Draws the country colors
                                }
                                else
                                {
                                    writeableBmp.FillRectangle(x, y, x+1, y+1, pixel); //Draws the province colors
                                }

                            }
                        }
            
            MainWindow.testimage2.Source = writeableBmp; */
        }

    }
}