using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static Paradox_Editor.ProgramProperties;

namespace Paradox_Editor
{
    public partial class MapEditor
    {
        //Above is currently useless: Use for graphical loading of map a later point.

        public static Bitmap ConvertToBitmap(BitmapSource bitmapSource)
        {
            var width = bitmapSource.PixelWidth;
            var height = bitmapSource.PixelHeight;
            var stride = width * ((bitmapSource.Format.BitsPerPixel + 7) / 8);
            var memoryBlockPointer = Marshal.AllocHGlobal(height * stride);
            bitmapSource.CopyPixels(new Int32Rect(0, 0, width, height), memoryBlockPointer, height * stride, stride);
            var bitmap = new Bitmap(width, height, stride, System.Drawing.Imaging.PixelFormat.Format32bppPArgb, memoryBlockPointer);
            return bitmap;
        }

        /// <summary>
        /// 
        /// - modname/commmon/countries.txt
        ///PSK		="countries/Pskov.txt"
        ///MAN		="countries/Mann.txt" *Utilize the filepath it ALREADY GIVES in the textfile
        ///JAN		="countries/Jan Mayen.txt"
        ///
        /// - modname/common/countries/Pskov.
        ///color = { 240  230  140 } *This is the country colour of Pskov indicated in the file
        ///
        /// 
        /// 
        /// - modname\history\provinces\africa\1688 - Ajdir.txt
        /// owner = SPA
        /// controller = SPA
        /// add_core = SPA
        /// add_core = MGH
        /// trade_goods = fish
        /// life_rating = 35
        /// colonial = 1
        /// 
        /// Country Colors file is for the soldier's uniforms.
        /// color1 - Part of uniform
        /// color2 - Part of uniform
        /// color3 - Part of uniform
        /// </summary>



        //Observable collections may not even be the way to go. Lists are simpler and more understood here
        //--------------------------------------------------------------------------
        public Country CurrentCountry { get; set; } //Acquires the data under ProvinceFile; ID, provinceName, Filepath
        //--------------------------------------------------------------------------
        public static ObservableCollection<Country> CountryList { get; set; } = new ObservableCollection<Country>(); //Connected to the XAML

        public static void TestProvinceUpdate() //The test process for how map loading should work.
        {

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

                    //Test next part in searching for country color in given filepath using respective origin\\
                    //Get specific array entry of "color = { 255  255  255 }" to represent the country's colour.\\

                    ///Provinces use ID, ProvinceRGB, ProvinceName; countries only use CountryName, CountryTAG, CountryRGB but
                    ///the program will need to read every provincefile according to the provinces, and display their RGB
                    ///according to the country that is set as "owner=" in said file. If no owner, then spit console error

                    string[] words = firstremoval.Split('=');

                    CountriesInTxt.Add(words); //CONTAINS country TAG & History txt file path

                }

                //CountryList.Add(new Country() { CountryName = values[4], ID = Int32.Parse(values[0]), RGB = values[1] + " " + " " + values[2] + " " + values[3] }); //Add the respective province data into the FilePaths source

            } //currently only reads (mostly) valid entries in the countries.txt file.
              //Next: get the color value of the given country (thanks to the embeded filepath)
              //- and then slap it together into the countrylist collection


            //Read TAG first
            //Listing would look like: { PSK, countries/Pskov.txt, 240  230  140 }
            //PSK.Path = countries/Pksov.txt
            //PSK.RGB = 240 230 140

            ///The tag PSK before path is a custom type: CountryTAG


            //Check owner & find tag that matches; get the tag's color


            List<string> insertedIDs = new List<string>();
            List<string[]> insertedRGBs = new List<string[]>();
            List<string> insertedPaths = new List<string>();
            List<string> insertedNames = new List<string>();

            var Paths = new string[] { "Path 1", "Path 2", "Path 3", "Path 4", "Path 5", "Path 6" };
            var TestCSVFile = new string[] { "1;255;0;0;Red", "2;38;0;255;Blue", "3;118;255;0;Green", "4;250;0;255;Purple", "5;0;242;255;Cyan", "6;250;255;0;Yellow" }; //Reads each entry from the filepath (the CSV File) as a part of an array


            //Code for the below already exists in the main MainWindow.xaml.cs, around Line 79\\
            foreach (var entry in TestCSVFile) //STRICTLY PROVINCE DATA ; USABLE FOR COUNTRY PLACEMENT | TEST CSV FILE\\
            {
                var values = entry.Split(';'); //Current values of said-line

                insertedIDs.Add(values[0]); //0 = ID, 1,2,3 = RGB, 4 = ProvinceName
                string[] strArr = { values[1] + " " + " " + values[2] + " " + values[3] }; //Adding RGB codes
                insertedNames.Add(values[4]); //Adding province name of current line to Province Name List
                insertedRGBs.Add(strArr); //Adds the RGB Array Values from current line into the Province RGB List

            }


            var MainWindow = (MainWindow)Application.Current.MainWindow;
            //MainWindow.testimage1.Source = new BitmapImage(new Uri("pack://application:,,,/Paradox Editor;component/TestProvince.bmp"));

            // Winforms Image we want to get the WPF Image from...

            ImageSource img = MainWindow.testimage1.Source;
            BitmapSource bmp = (BitmapSource)img;

            var dzungar = ConvertToBitmap(bmp);

            for (int x = 0; x < dzungar.Width; x++)
            {
                for (int y = 0; y < dzungar.Height; y++)
                {
                    System.Drawing.Color pixel = dzungar.GetPixel(x, y); //x = 94, y = 38 (255, 0, 00) ; RED
                    int red = pixel.R;
                    int green = pixel.G;
                    int blue = pixel.B;
                }
            }


            //\\

        }

    }
}