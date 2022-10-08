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
        private DataAcquisition ModData { get; set; }
        public ProvinceFile SelectedItem { get; set; } //Acquires the data under ProvinceFile; ID, provinceName, Filepath
        public int CurrentControlMode { get; set; }
        public int CurrentGameMode { get; set; }


        /// <summary>
        /// Numerous calls, constructors and methods to allow Province Data to bind to the History-File-List DataGrid.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChange(string propertyName)
        { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); }
        //Crashes after loading second mod
        private ObservableCollection<ProvinceFile> _provincedata = new();
        public ObservableCollection<ProvinceFile> ProvinceData
        { get => _provincedata; set { _provincedata = value; NotifyPropertyChange(nameof(ProvinceData)); } }

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            /*            var timer = new DispatcherTimer();
                        timer.Interval = TimeSpan.FromSeconds(0.01);
                        timer.Tick += new EventHandler(Navigator.MoveTimerTick);
                        timer.Start();*/
        }

        private void MainWindow_Load(object _1, EventArgs _2)
        {
            VisualHandler.ConductAssetChange("VIC2");
            mapModeButtons.UpdateMapModeVisibility(1, VisualHandler.MapModeIconSet);
        }



        public void SelectMasterFolder_Click(object sender, EventArgs e)
        {
            //var MainWindow = (MainWindow)Application.Current.MainWindow; //May be useless
            
            Explorer filesector = new();
            string selectedDirectory = filesector.OpenFolderSelect();
            if (selectedDirectory == null)
                return;

            
            DataAcquisition provinceData = new(selectedDirectory);
            ModData = provinceData.ReturnAcquisitionAllData(selectedDirectory);

            ///For the History File Lister
            ProvinceData = new ObservableCollection<ProvinceFile>(ModData.GetProvinces().Values);
            fileListView.ItemsSource = ProvinceData;

            provinceInterface.SetModData(ModData);
            mapViewer.SetModData(ModData);
            mapViewer.LoadMaps();
            

        }

        public void OpenFileFromList(object sender, RoutedEventArgs e)
        { Explorer fileSelector = new(); fileSelector.OpenFile(SelectedItem.HistoryFilePath); }

        public static explicit operator MainWindow(WindowCollection v) => throw new NotImplementedException();

        public void SelectGame(object sender, RoutedEventArgs e)
        {
            var game = GameSelectDropdown.SelectedItem.ToString();
            switch (game)
            {
                case string when game.Contains("Victoria II"): //Problem: It is ignoring state-buildings!
                    SoundHandler.SoundAssetChange("VIC2");
                    VisualHandler.ConductAssetChange("VIC2");
                    break;
                case string when game.Contains("Europa Universalis IV"):
                    SoundHandler.SoundAssetChange("EU4");
                    VisualHandler.ConductAssetChange("EU4");
                    break;
                default:
                    SoundHandler.SoundAssetChange("VIC2");
                    VisualHandler.ConductAssetChange("VIC2");
                    break;
            }

            provinceInterface.UpdateUI();
            mapModeButtons.UpdateMapModeVisibility(mapModeButtons.GetMapMode(), VisualHandler.MapModeIconSet);
            SoundHandler.PlayConnectingSound();
        }

        private void ChangeControlScheme(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            CurrentControlMode = ControlMode.SelectedIndex;
        }


    }
}
