using System.Windows.Media;
using System.Windows.Controls;
using System.Collections.Generic;
using Paradox_Editor.D_Types;
using Paradox_Editor.B_Data_Functions;
using Paradox_Editor.A_All_New_Methods;

namespace Paradox_Editor.C_Window_Functions
{

    public class FolderSelect
    {
        public static bool IsMapLoaded; //Obselete, Move to Map Renderer

        //public static string StoredOpener { get; set; } = Path.Combine("E:", "Games", "Victoria II", "mod", "LZ22");
        //Stored opener is subject to change for user convienence
        public Dictionary<uint, string> StoredProvinceColorToID { get; set; } //Obselete, Delete
        public Dictionary<string, string> StoredTagToCountryName { get; set; } //Obselete, Delete
        public Dictionary<string, Color> StoredCountryNameToColor { get; private set; } //Obselete, Delete
        public ProvinceOutputData StoredProvinceIDToDataDictionaries { get; private set; } //Obselete, Delete
        public DataAcquisition ModData { get; set; } //Check if type needs rename.

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

            Explorer filesector = new();
            string selectedDirectory = filesector.OpenFileSelect();
            if (selectedDirectory == null)
                return;
            

            IsMapLoaded = true;

            DataAcquisition provinceData = new(selectedDirectory);
            ModData = provinceData.ReturnAcquisitionAllData(selectedDirectory);
            /*
            //✓✓✓
            StoredProvinceIDToDataDictionaries = provinceIDToDataDictionaries;
            //✓✓✓
            var tagToCountryName = dataAcquisitionInstance.GetTagToCountryName(directoryData.CountriesTxt);
            StoredTagToCountryName = tagToCountryName;
            //✓✓✓
            CountryNameToColor CountryNameToColorConverter = new();
            var countryNameToColor = CountryNameToColorConverter.GetCountryColor(directoryData.Countries);
            StoredCountryNameToColor = countryNameToColor; //Check if used
            //✓✓✓
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
            ///---------------------------------------------------------------*/
        }
    }
}
