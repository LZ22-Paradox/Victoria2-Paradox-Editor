using Paradox_Editor.Extensions;
using Paradox_Editor.Extensions.Types;
using Paradox_Editor.Handlers;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Forms;

//F1 to see WIKI detail on part
//F12 to see usage in VS
//CTRL +press+ K, D sorts all tabs

namespace Paradox_Editor
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public ProvinceFile SelectedItem { get; set; } //Acquires the data under ProvinceFile; ID, provinceName, Filepath
        public int CurrentControlMode { get; set; }
        public static string CurrentGameMode { get; set; }

        #region Calls / constructors methods allowing Province Data to bind to the History-File-List DataGrid.
        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChange(string propertyName)
        { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); }
        private ObservableCollection<ProvinceFile> _provincedata = new();
        public ObservableCollection<ProvinceFile> ProvinceData
        { get => _provincedata; set { _provincedata = value; NotifyPropertyChange(nameof(ProvinceData)); } }
        #endregion

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        private void MainWindow_Load(object _1, EventArgs _2)
        {
            VisualHandler.ConductAssetChange("VIC2");
            mapModeButtons.UpdateMapModeVisibility(1, VisualHandler.MapModeIconSet);
        }


        public void SelectMasterFolder_Click(object sender, EventArgs e)
        {
            //var MainWindow = (MainWindow)Application.Current.MainWindow; //May be useless

            string selectedModFolder = Explorer.OpenFolderSelect();
            if (selectedModFolder == null)
                return;

            Thread thread = new Thread(() => ModData.AcquisitionData(selectedModFolder));
            thread.Start();
            thread.Join();

            //For the History File Lister
            ProvinceData = new ObservableCollection<ProvinceFile>(ModData.PROVINCE_DATA.GetProvinces().Values);
            fileListView.ItemsSource = ProvinceData;

            mapViewer.LoadMaps();
            popAdjuster.Update();
            provinceInterface.Update();
		}

        public void OpenFileFromList(object sender, RoutedEventArgs e)
        {
            Explorer.OpenFile(SelectedItem.HistoryFilePath);
        }

        public static explicit operator MainWindow(WindowCollection v) => throw new NotImplementedException();

        public void SelectGame(object sender, RoutedEventArgs e)
        {
            var game = GameSelectDropdown.SelectedItem.ToString();
            switch (game)
            {
                case string when game.Contains("Victoria II"):
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
            SoundHandler.PlayConnecting();
        }

        private void ChangeControlScheme(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            CurrentControlMode = ControlMode.SelectedIndex;
        }

        private void provinceInterface_Loaded(object sender, RoutedEventArgs e)
        {

        }

    }
}
