using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using Paradox_Editor.A_Map_Functions;
using Paradox_Editor.C_Window_Functions;
using Paradox_Editor.D_Static_Classes_Types;

//F1 to see WIKI detail on part
//F12 to see usage in VS
//CTRL +press+ K, D sorts all tabs

namespace Paradox_Editor
{
    public partial class MainWindow : Window
    {
        private MapNavigation Navigator;
        private FolderSelect SelectMap;

        public static ProvinceFile SelectedItem { get; set; } //Acquires the data under ProvinceFile; ID, provinceName, Filepath
        public static ObservableCollection<ProvinceFile> ProvinceData { get; set; } = new ObservableCollection<ProvinceFile>();

        public MainWindow()
        {
            InitializeComponent();

            Navigator =
                new MapNavigation(mapCanvas)
                .AddImage(mapProvinces)
                .AddImage(mapPolitical)
                .AddImage(testimage2);

            SelectMap = new FolderSelect(mapCanvas, mapProvinces, mapPolitical);
        }

        private void MainWindow_Load(object _1, EventArgs _2)
        {
            MapModesControl.CurrentMapMode = 1;
            MenuVisualHandler.VisualHandler.IconAssetsPath = @"/Preloaded_Assets/VIC2/Icons/";
            MapModesControl.UpdateMapModeVisibility();
        }

        public void Map_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Navigator.MouseLeftButtonUp(sender, e);
        }

        public void Map_MouseLeave(object sender, MouseEventArgs e)
        {
            Navigator.MouseLeave(sender, e);
        }

        public void Map_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Navigator.MouseLeftButtonDown(sender, e);
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

        public void MapClick(object sender, RoutedEventArgs e) //Testing to get position of the mouse
        {
            Debug.WriteLine(Mouse.GetPosition(Mouse.DirectlyOver));
            Debug.WriteLine("Reading");
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
                MenuVisualHandler.VisualHandler.ConductAssetChange("VIC2");
                MapModesControl.UpdateMapModeVisibility();
            }
            else if (GameSelect.SelectedItem.ToString().Contains("Europa Universalis IV"))
            {
                GameSoundHandler.SoundHandler.SoundAssetChange("EU4");
                MenuVisualHandler.VisualHandler.ConductAssetChange("EU4");
                MapModesControl.UpdateMapModeVisibility();
            } else
            {
                GameSoundHandler.SoundHandler.SoundAssetChange("VIC2");
            }
            GameSoundHandler.SoundHandler.PlayConnectingSound();
        }
    }
}
