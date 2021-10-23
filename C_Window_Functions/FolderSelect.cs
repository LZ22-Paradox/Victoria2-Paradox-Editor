using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Media;
using Point = System.Windows.Point;
using System.Windows.Controls;
using Paradox_Editor.B_Map_Functions;
using System.Windows.Forms;
using MessageBox = System.Windows.Forms.MessageBox;
using Paradox_Editor.D_Static_Classes_Types;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using Paradox_Editor.A_Map_Navigation;
using System.Windows.Shell;
using System.Windows;
using System.Threading;

namespace Paradox_Editor.C_Window_Functions
{


    public class FolderSelect
    {

        public static string StoredOpener { get; set; } = Path.Combine("E:", "Games", "Victoria II", "mod", "LZ22");
        private Canvas Canvas;
        private Image Image1;
        private Image Image2;
        //Stored opener will be subject to change for user convienence

        public FolderSelect(Canvas canvas, Image image1, Image image2)
        {
            Canvas = canvas;
            Image1 = image1;
            Image2 = image2;
        }

        public FolderSelect() { }

        public void SelectMainFolder()
        {
            var MainWindow = (MainWindow)System.Windows.Application.Current.MainWindow;

//-------------------------------Split Into New Method---------------------------------------------
            var dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() != DialogResult.OK)
                return;

            MessageBox.Show("You selected Filepath: " + dialog.SelectedPath);
            Console.WriteLine(dialog.SelectedPath);
            var selectDirectory = dialog.SelectedPath;
            var storedOpener = dialog.SelectedPath; //Unused; Reimpliment stored opener.
            Console.ReadLine();
            MainWindow.ProvinceData.Clear();
//-------------------------------Split Into New Method----------------------------------------------


            var mapEditorInstance = new DataAcquisition();
            var directoryData = mapEditorInstance.CollectDirectoryData(selectDirectory);

            var provinceColorToID = mapEditorInstance.GetProvinceColorToID(directoryData.DefinitionCSV);
            var tagToCountryName = mapEditorInstance.GetTagToCountryName(directoryData.CountriesTxt);
            var provinceIDToDataDictionaries = mapEditorInstance.GetProvinceIDToData(directoryData.HistoryProvinces);


            CountryNameToColor CountryNameToColorConverter = new CountryNameToColor();
            var countryNameToColor = CountryNameToColorConverter.GetCountryColor(directoryData.Countries);

            var provinceDataCollection = new TextFileExtractor(directoryData.HistoryProvinces, MainWindow.ProvinceData);
            MainWindow.ProvinceData = provinceDataCollection.ExtractForCollection(directoryData.HistoryProvinces); //Responsible for Left Panel

            MainWindow.fileListView.ItemsSource = MainWindow.ProvinceData;

            var imgs = new ImageSourceConverter(); //Create instance of the image converter
            var flipTrans = new ScaleTransform(); //creates instance for scale
            Canvas.RenderTransformOrigin = new Point(0.5, 0.5); //Sets the origin/middle point of the new image
            flipTrans.ScaleY = -1; //flip the scale of the Y (horizontal) so it is the right side up
            Canvas.RenderTransform = flipTrans; //Actually render the changes

            Image1.SetValue(Image.SourceProperty, imgs.ConvertFromString(Path.Combine(dialog.SelectedPath, "map", "provinces.bmp")));
            Image2 = Image1;



            ///-----------------------Lazy Ending-----------------------------
            var firstLayer = BitmapFactory.ConvertToPbgra32Format((BitmapSource)MainWindow.mapProvinces.Source); //May be problem
            var writeableBmp = BitmapFactory.New(firstLayer.PixelWidth, firstLayer.PixelHeight); //Different dimensions than firstlayer
            writeableBmp.Clear(Colors.White);
            var ColorMap = new MapRenderer(firstLayer, writeableBmp, MainWindow.mapProvinces, provinceColorToID, provinceIDToDataDictionaries.IDToController, tagToCountryName, countryNameToColor);
            ColorMap.Drawing();
            MainWindow.mapPolitical.Source = ColorMap.SizeReference;
            ///---------------------------------------------------------------


        }
    }
}
