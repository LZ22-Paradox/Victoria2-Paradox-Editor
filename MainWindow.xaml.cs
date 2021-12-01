using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Paradox_Editor.A_Map_Functions;
using Paradox_Editor.C_Window_Functions;
using Paradox_Editor.D_Class_Types;
using Point = System.Drawing.Point;

//F1 to see WIKI detail on part
//F12 to see usage in VS
//CTRL +press+ K, D sorts all tabs

namespace Paradox_Editor
{
    public partial class MainWindow : Window
    {
        private NavigationHandler Navigator;
        public FolderSelect SelectMap;

        public static ProvinceFile SelectedItem { get; set; } //Acquires the data under ProvinceFile; ID, provinceName, Filepath
        public int CurrentControlMode { get; set; } //Acquires the data under ProvinceFile; ID, provinceName, Filepath

        public static ObservableCollection<ProvinceFile> ProvinceData { get; set; } = new ObservableCollection<ProvinceFile>();

        public MainWindow()
        {
            InitializeComponent();

            Navigator =
                new NavigationHandler(mapCanvas)
                .AddImage(mapProvinces)
                .AddImage(mapPolitical)
                .AddImage(mapTerrain);

            SelectMap = new FolderSelect(mapCanvas, mapProvinces, mapPolitical);

            this.KeyDown += new KeyEventHandler(Navigator.KeyPressed);
            //The above May be changed from map provinces to grid.
        }

        private void MainWindow_Load(object _1, EventArgs _2)
        {
            MapModesControl.CurrentGameMode = MenuVisualHandler.VisualHandler.ConductAssetChange("VIC2");
            MapModesControl.UpdateMapModeVisibility(1, MapModesControl.CurrentGameMode);
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
            if (CurrentControlMode == 0)
            {
                if (e.MiddleButton == MouseButtonState.Pressed)
                {
                    Navigator.MouseDown(sender, e);
                }
                else if (e.LeftButton == MouseButtonState.Pressed)
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
            /*            Task taskboi = new Task(SelectMap.SelectMainFolder); //Not entirely functional. Fix.
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
                MapModesControl.CurrentGameMode = MenuVisualHandler.VisualHandler.ConductAssetChange("VIC2");

                MapModesControl.UpdateMapModeVisibility(MapModesControl.CurrentMapMode, MapModesControl.CurrentGameMode);
            }
            else if (GameSelect.SelectedItem.ToString().Contains("Europa Universalis IV"))
            {
                GameSoundHandler.SoundHandler.SoundAssetChange("EU4");
                MapModesControl.CurrentGameMode = MenuVisualHandler.VisualHandler.ConductAssetChange("EU4");
                MapModesControl.UpdateMapModeVisibility(MapModesControl.CurrentMapMode, MapModesControl.CurrentGameMode);
            }
            else
            {
                GameSoundHandler.SoundHandler.SoundAssetChange("VIC2");
            }
            GameSoundHandler.SoundHandler.PlayConnectingSound();
        }

        private void ControlChange(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            CurrentControlMode = ControlSelect.SelectedIndex;

        }


    }
}
