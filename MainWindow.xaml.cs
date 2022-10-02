using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Paradox_Editor.A_All_New_Methods;
using Paradox_Editor.A_Map_Functions;
using Paradox_Editor.C_Window_Functions;
using Paradox_Editor.D_Types;

//F1 to see WIKI detail on part
//F12 to see usage in VS
//CTRL +press+ K, D sorts all tabs

namespace Paradox_Editor
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public DataAcquisition ModData { get; set; }

        private NavigationHandler Navigator;
        public static bool IsMapLoaded; //Obselete, Move to Map Renderer

        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChange(string propertyName)
        { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); }
        //Crashes after loading second mod

        public ProvinceFile SelectedItem { get; set; } //Acquires the data under ProvinceFile; ID, provinceName, Filepath
        public static bool IsImageFlipped { get; set; }
        public int CurrentControlMode { get; set; } //Acquires the data under ProvinceFile; ID, provinceName, Filepath
        public int CurrentGameMode { get; set; } //Acquires the data under ProvinceFile; ID, provinceName, Filepath

        private ObservableCollection<ProvinceFile> _provincedata = new();
        public ObservableCollection<ProvinceFile> ProvinceData
        { get => _provincedata; set { _provincedata = value; NotifyPropertyChange("ProvinceData"); } }

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;

            Navigator =
                new NavigationHandler(mapCanvas)
                .AddImage(mapProvinces)
                .AddImage(mapPolitical)
                .AddImage(mapTerrain);

            /*            var timer = new DispatcherTimer();
                        timer.Interval = TimeSpan.FromSeconds(0.01);
                        timer.Tick += new EventHandler(Navigator.MoveTimerTick);
                        timer.Start();*/
        }

        private void MainWindow_Load(object _1, EventArgs _2)
        {
            VisualHandler.ConductAssetChange("VIC2");
            MapModesControl.UpdateMapModeVisibility(1, VisualHandler.MapModeIconSet);
        }

        public void Map_MouseUp(object sender, MouseButtonEventArgs e)
        { Navigator.ReleaseMouseCapture(); }

        public void Map_MouseLeave(object sender, MouseEventArgs e)
        { Navigator.MouseLeave(sender, e); }

        public void Map_MouseDown(object sender, MouseButtonEventArgs e)
        {

            if (e.MiddleButton.Equals(MouseButtonState.Pressed)) //This is for the alternate types of interactions w. the map
            {
                Navigator.MouseDown(sender, e);
            }
            else if (e.LeftButton.Equals(MouseButtonState.Pressed) && IsMapLoaded)
            {
                if (mapModeButtons.CurrentMapMode == 0) //Political Mapmode
                {
                    //null 
                }
                if (mapModeButtons.CurrentMapMode == 1) //Province Mapmode
                {
                    Navigator.MouseLeftClick(sender, e);
                }
            }

        }

        public void Map_MouseWheel(object sender, MouseWheelEventArgs e)
        { Navigator.MouseWheel(sender, e); }

        public void Map_MouseMove(object sender, MouseEventArgs e)
        { Navigator.MouseMove(sender, e); }

        public void SelectMasterFolder_Click(object sender, EventArgs e)
        {
            var MainWindow = (MainWindow)System.Windows.Application.Current.MainWindow;

            Explorer filesector = new();
            string selectedDirectory = filesector.OpenFolderSelect();
            if (selectedDirectory == null)
                return;

            IsMapLoaded = true; //Move AFTER map is rendered?
            DataAcquisition provinceData = new(selectedDirectory);
            ModData = provinceData.ReturnAcquisitionAllData(selectedDirectory);

            ///For the History File Lister
            ProvinceData = new ObservableCollection<ProvinceFile>(ModData.GetProvinces().Values);
            fileListView.ItemsSource = ProvinceData;

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
            ///--*/
        }

        public void OpenFileFromList(object sender, RoutedEventArgs e)
        { Explorer fileSelector = new(); fileSelector.OpenFile(SelectedItem.HistoryFilePath); }

        public static explicit operator MainWindow(WindowCollection v) => throw new NotImplementedException();

        public void SelectGame(object sender, RoutedEventArgs e)
        {
            if (GameSelectDropdown.SelectedItem.ToString().Contains("Victoria II"))
            {
                SoundHandler.SoundAssetChange("VIC2");
                VisualHandler.ConductAssetChange("VIC2");

                MapModesControl.UpdateMapModeVisibility(this.mapModeButtons.CurrentMapMode, VisualHandler.MapModeIconSet);
            }
            else if (GameSelectDropdown.SelectedItem.ToString().Contains("Europa Universalis IV"))
            {
                SoundHandler.SoundAssetChange("EU4");
                VisualHandler.ConductAssetChange("EU4");
                MapModesControl.UpdateMapModeVisibility(this.mapModeButtons.CurrentMapMode, VisualHandler.MapModeIconSet);
            }
            else
            {
                SoundHandler.SoundAssetChange("VIC2");
                VisualHandler.ConductAssetChange("VIC2");
            }

            SoundHandler.PlayConnectingSound();
        }

        private void ChangeControlScheme(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            CurrentControlMode = ControlMode.SelectedIndex;

        }
    }
}
