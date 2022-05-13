using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Paradox_Editor.A_Map_Functions;
using Paradox_Editor.C_Window_Functions;
using Paradox_Editor.D_Types;

//F1 to see WIKI detail on part
//F12 to see usage in VS
//CTRL +press+ K, D sorts all tabs

namespace Paradox_Editor
{
    public partial class MainWindow : Window
    {
        private NavigationHandler Navigator;
        public FolderSelect SelectMap;
        public ProvinceFile SelectedItem { get; set; } //Acquires the data under ProvinceFile; ID, provinceName, Filepath
        public static bool IsImageFlipped { get; set; }
        public int CurrentControlMode { get; set; } //Acquires the data under ProvinceFile; ID, provinceName, Filepath
        public int CurrentGameMode { get; set; } //Acquires the data under ProvinceFile; ID, provinceName, Filepath

        public static ObservableCollection<ProvinceFile> ProvinceData { get; set; } = new ObservableCollection<ProvinceFile>();
        public static int InterfaceActualRows { get; set; }
        public MainWindow()
        {
            InitializeComponent();

            InterfaceActualRows = FileInterface.ProvinceInterfaceViewerGrid.RowDefinitions.Count;

            Navigator =
                new NavigationHandler(mapCanvas)
                .AddImage(mapProvinces)
                .AddImage(mapPolitical)
                .AddImage(mapTerrain);

            SelectMap = new FolderSelect(mapCanvas, mapProvinces, mapPolitical);


            var timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(0.01);
            timer.Tick += new EventHandler(Navigator.MoveTimerTick);
            timer.Start();
        }




        private void MainWindow_Load(object _1, EventArgs _2)
        {
            MapModesControl.CurrentGameAssets = MenuVisualHandler.VisualHandler.ConductAssetChange("VIC2");
            MapModesControl.UpdateMapModeVisibility(1, MapModesControl.CurrentGameAssets.MapModeIconSet);
        }

        public void Map_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Navigator.ReleaseMouseCapture();
        }

        public void Map_MouseLeave(object sender, MouseEventArgs e)
        {
            Navigator.MouseLeave(sender, e);
        }

        public void Map_MouseDown(object sender, MouseButtonEventArgs e)
        {

            if (e.MiddleButton.Equals(MouseButtonState.Pressed)) //This is for the alternate types of interactions w. the map
            {
                Navigator.MouseDown(sender, e);
            }
            else if (e.LeftButton.Equals(MouseButtonState.Pressed) && FolderSelect.IsMapLoaded)
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
        {
            Navigator.MouseWheel(sender, e);
        }

        public void Map_MouseMove(object sender, MouseEventArgs e)
        {
            Navigator.MouseMove(sender, e);
        }

        public void SelectMasterFolder_Click(object sender, EventArgs e)
        {
            /*            Task taskboi = new Task(SelectMap.SelectMainFolder);
             *            //Not entirely functional. Should Fix.
                        taskboi.Start();*/
            SelectMap.SelectMainFolder();
        }

        public void OpenFileFromList(object sender, RoutedEventArgs e)
        {
            var fileopener = new Process();
            fileopener.StartInfo.FileName = "explorer";
            fileopener.StartInfo.Arguments = SelectedItem.FilePath;
            fileopener.Start();
        }

        public static explicit operator MainWindow(WindowCollection v)
        {
            throw new NotImplementedException();
        }

        public void GameSelected(object sender, RoutedEventArgs e)
        {
            var VisualHandler = new MenuVisualHandler();
            if (GameSelect.SelectedItem.ToString().Contains("Victoria II"))
            {
                GameSoundHandler.SoundHandler.SoundAssetChange("VIC2");
                MapModesControl.CurrentGameAssets = MenuVisualHandler.VisualHandler.ConductAssetChange("VIC2");

                MapModesControl.UpdateMapModeVisibility(this.mapModeButtons.CurrentMapMode, MapModesControl.CurrentGameAssets.MapModeIconSet);
                VisualHandler.UpdateInterface(MenuVisualHandler.VisualHandler.ConductAssetChange("VIC2"));
            }
            else if (GameSelect.SelectedItem.ToString().Contains("Europa Universalis IV"))
            {
                GameSoundHandler.SoundHandler.SoundAssetChange("EU4");
                MapModesControl.CurrentGameAssets = MenuVisualHandler.VisualHandler.ConductAssetChange("EU4");
                VisualHandler.UpdateInterface(MenuVisualHandler.VisualHandler.ConductAssetChange("EU4"));

                MapModesControl.UpdateMapModeVisibility(this.mapModeButtons.CurrentMapMode, MapModesControl.CurrentGameAssets.MapModeIconSet);
            }
            else
            {
                VisualHandler.UpdateInterface(MenuVisualHandler.VisualHandler.ConductAssetChange("VIC2"));
                GameSoundHandler.SoundHandler.SoundAssetChange("VIC2");
            }
            GameSoundHandler.SoundHandler.PlayConnectingSound();
        }

        private void ControlChange(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            CurrentControlMode = ControlMode.SelectedIndex;

        }
    }
}
