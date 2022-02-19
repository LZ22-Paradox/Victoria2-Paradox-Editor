using System.IO;
using System.Windows.Media;
using System.Windows.Controls;
using Paradox_Editor.B_Map_Functions;
using System.Windows.Media.Imaging;
using Paradox_Editor.A_Map_Navigation;
using Paradox_Editor.A_Map_Functions;
using System.Collections.Generic;
using Paradox_Editor.D_Types;

namespace Paradox_Editor.C_Window_Functions
{

    public class FolderSelect
    {
        public static bool IsMapLoaded;

        //public static string StoredOpener { get; set; } = Path.Combine("E:", "Games", "Victoria II", "mod", "LZ22");
        //Stored opener is subject to change for user convienence
        public string StoredGameDirectory { get; set; }
        public Dictionary<uint, string> StoredProvinceColorToID { get; set; }
        public Dictionary<string, string> StoredTagToCountryName { get; set; }
        public Dictionary<string, Color> StoredCountryNameToColor { get; private set; }
        public ProvinceOutputData StoredProvinceIDToDataDictionaries { get; private set; }

        private Canvas Canvas;
        private Image Image1;
        private Image Image2;

        public FolderSelect(Canvas canvas, Image image1, Image image2)
        {
            Canvas = canvas;
            Image1 = image1;
            Image2 = image2;
        }

        public FolderSelect() { }

        //GameSoundHandler.SoundHandler.PlayConnectingSound();

        public void SelectMainFolder()
        {
            var MainWindow = (MainWindow)System.Windows.Application.Current.MainWindow;

            var filesector = new Explorer();
            var selectedDirectory = filesector.OpenFileSelect();
            StoredGameDirectory = selectedDirectory;
            if (selectedDirectory == null)
            {
                return;
            }

            IsMapLoaded = true;
            var dataAcquisitionInstance = new FileDataAcquisition();
            var directoryData = dataAcquisitionInstance.CollectDirectoryData(selectedDirectory);

            var provinceColorToID = dataAcquisitionInstance.GetProvinceColorToID(directoryData.DefinitionCSV);
            StoredProvinceColorToID = provinceColorToID;

            var provinceIDToDataDictionaries = dataAcquisitionInstance.GetProvinceIDToData(directoryData.HistoryProvinces);
            StoredProvinceIDToDataDictionaries = provinceIDToDataDictionaries;

            var tagToCountryName = dataAcquisitionInstance.GetTagToCountryName(directoryData.CountriesTxt);
            StoredTagToCountryName = tagToCountryName;

            CountryNameToColor CountryNameToColorConverter = new();
            var countryNameToColor = CountryNameToColorConverter.GetCountryColor(directoryData.Countries);
            StoredCountryNameToColor = countryNameToColor;
            
            var provinceDataCollection = new DataExtractor(directoryData.HistoryProvinces, MainWindow.ProvinceData);
            MainWindow.ProvinceData = provinceDataCollection.ExtractForCollection(directoryData.HistoryProvinces); //Responsible for Left Panel

            MainWindow.fileListView.ItemsSource = MainWindow.ProvinceData;

            var image = new ImageTransformation(Canvas);
            image.InvertCanvas(Canvas);

            var imgs = new ImageSourceConverter(); //Create instance of the image converter
            Image1.SetValue(Image.SourceProperty, imgs.ConvertFromString(Path.Combine(selectedDirectory, "map", "provinces.bmp")));
            Image2 = Image1;
            
            ///-----------------------Lazy Ending-----------------------------
            var firstLayer = BitmapFactory.ConvertToPbgra32Format((BitmapSource)MainWindow.mapProvinces.Source); //May be problem
            var writeableBmp = BitmapFactory.New(firstLayer.PixelWidth, firstLayer.PixelHeight); //Different dimensions than firstlayer
            writeableBmp.Clear(Colors.White);
            var ColorMap = new MapRenderer(firstLayer, writeableBmp, MainWindow.mapProvinces,
                provinceColorToID, provinceIDToDataDictionaries.IDToOwner,
                tagToCountryName, countryNameToColor);
            ColorMap.DrawProvinceMap();
            MainWindow.mapPolitical.Source = ColorMap.SizeReference;
            ///---------------------------------------------------------------
        }
    }
}
