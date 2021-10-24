using System.IO;
using System.Windows.Media;
using System.Windows.Controls;
using Paradox_Editor.B_Map_Functions;
using System.Windows.Media.Imaging;
using Paradox_Editor.A_Map_Navigation;
using Paradox_Editor.A_Map_Functions;
using System.Threading;
using System.Threading.Tasks;

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


        //GameSoundHandler.SoundHandler.PlayConnectingSound();

        public void SelectMainFolder()
        {
            var MainWindow = (MainWindow)System.Windows.Application.Current.MainWindow;

            var filesector = new Explorer();
            var selectedDirectory = filesector.OpenFileSelect(); if (selectedDirectory == null)
            {
                return;
            }

            var dataAcquisitionInstance = new DataAcquisition();
            var directoryData = dataAcquisitionInstance.CollectDirectoryData(selectedDirectory);

            var provinceColorToID = dataAcquisitionInstance.GetProvinceColorToID(directoryData.DefinitionCSV);
            var tagToCountryName = dataAcquisitionInstance.GetTagToCountryName(directoryData.CountriesTxt);
            var provinceIDToDataDictionaries = dataAcquisitionInstance.GetProvinceIDToData(directoryData.HistoryProvinces);


            CountryNameToColor CountryNameToColorConverter = new();
            var countryNameToColor = CountryNameToColorConverter.GetCountryColor(directoryData.Countries);

            var provinceDataCollection = new TextFileExtractor(directoryData.HistoryProvinces, MainWindow.ProvinceData);
            MainWindow.ProvinceData = provinceDataCollection.ExtractForCollection(directoryData.HistoryProvinces); //Responsible for Left Panel


            MainWindow.fileListView.ItemsSource = MainWindow.ProvinceData;

            var image = new ImageTransformation(Canvas);
            image.InvertImage(Canvas);

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
            ColorMap.Drawing();
            MainWindow.mapPolitical.Source = ColorMap.SizeReference;
            ///---------------------------------------------------------------
        }
    }
}
