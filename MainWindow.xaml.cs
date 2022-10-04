using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Paradox_Editor.A_All_New_Methods;
using Paradox_Editor.A_Map_Functions;
using Paradox_Editor.A_Map_Navigation;
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
        public static bool IsMapLoaded;

        public ProvinceFile SelectedItem { get; set; } //Acquires the data under ProvinceFile; ID, provinceName, Filepath
        public static bool IsImageFlipped { get; set; } //Possibly may be useless
        public int CurrentControlMode { get; set; }
        public int CurrentGameMode { get; set; }

        public Dictionary<int, Image> MapModes { get; set; }

        /// <summary>
        /// Numerous calls, constructors and methods to allow Province Data to bind to the History-File-List DataGrid.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChange(string propertyName)
        { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); }
        //Crashes after loading second mod
        private ObservableCollection<ProvinceFile> _provincedata = new();
        public ObservableCollection<ProvinceFile> ProvinceData
        { get => _provincedata; set { _provincedata = value; NotifyPropertyChange("ProvinceData"); } }

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;

            MapModes = new Dictionary<int, Image>();
            MapModes.Add(0, mapProvinces);
            MapModes.Add(1, mapPolitical);
            MapModes.Add(2, mapTerrain);
            Navigator = new NavigationHandler(mapCanvas, MapModes);

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
                Navigator.MouseLeftClick(sender, e);
            }

        }

        public void Map_MouseWheel(object sender, MouseWheelEventArgs e)
        { Navigator.MouseWheel(sender, e); }

        public void Map_MouseMove(object sender, MouseEventArgs e)
        { Navigator.MouseMove(sender, e); }

        public void SelectMasterFolder_Click(object sender, EventArgs e)
        {
            var MainWindow = (MainWindow)Application.Current.MainWindow; //May be useless
            
            Explorer filesector = new();
            string selectedDirectory = filesector.OpenFolderSelect();
            if (selectedDirectory == null)
                return;

            
            DataAcquisition provinceData = new(selectedDirectory);
            ModData = provinceData.ReturnAcquisitionAllData(selectedDirectory);

            ///For the History File Lister
            ProvinceData = new ObservableCollection<ProvinceFile>(ModData.GetProvinces().Values);
            fileListView.ItemsSource = ProvinceData;


            ///Load Province Map
            //Untested Code - Fix Up : Provinces map dissapears when re-selected : Move to MapRenderer class
            var image = new ImageTransformation(mapCanvas);
            image.InvertCanvas();
            var imgs = new ImageSourceConverter(); //Create instance of the image converter
            mapProvinces.SetValue(Image.SourceProperty, imgs.ConvertFromString(Path.Combine(selectedDirectory, "map", "provinces.bmp")));

            ///Load Political Map
            var provinceMapSource = BitmapFactory.ConvertToPbgra32Format((BitmapSource)MainWindow.mapProvinces.Source); //May be problem
            //var writableImage = BitmapFactory.New(provinceMapSource.PixelWidth, provinceMapSource.PixelHeight); //Different dimensions than firstlayer
            //writableImage.Clear(Colors.White); //Clears an image. Could be useful.
            var politicalMap = new MapRenderer(ModData).DrawPoliticalMap(provinceMapSource);
            MainWindow.mapPolitical.Source = politicalMap;

            ///Load D_ Map


            ///Load D_ Map


            IsMapLoaded = true;
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

                MapModesControl.UpdateMapModeVisibility(this.mapModeButtons.GetMapMode(), VisualHandler.MapModeIconSet);
            }
            else if (GameSelectDropdown.SelectedItem.ToString().Contains("Europa Universalis IV"))
            {
                SoundHandler.SoundAssetChange("EU4");
                VisualHandler.ConductAssetChange("EU4");
                MapModesControl.UpdateMapModeVisibility(this.mapModeButtons.GetMapMode(), VisualHandler.MapModeIconSet);
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
