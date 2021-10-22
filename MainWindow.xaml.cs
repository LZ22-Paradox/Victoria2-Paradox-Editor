using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Paradox_Editor.C_Window_Functions;
using Paradox_Editor.D_Static_Classes_Types;

//F1 to see WIKI detail on part
//F12 to see mechanicla usage in VS
//CTRL +press+ K, D sorts all tabs

namespace Paradox_Editor
{
    public partial class MainWindow : Window
    {
        private MapNavigation Navigator;
        private FolderSelect SelectMap;
        public static ProvinceFile SelectedItem { get; set; } //Acquires the data under ProvinceFile; ID, provinceName, Filepath
        public static ObservableCollection<ProvinceFile> ProvinceData { get; set; } = new ObservableCollection<ProvinceFile>();


        //private MapEditor UpdateTest;
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
            MapModesControl.updateMapModeVisibility();
        }

        public void map_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Navigator.MouseLeftButtonUp(sender, e);
        }

        public void map_MouseLeave(object sender, MouseEventArgs e)
        {
            Navigator.MouseLeave(sender, e);
        }

        public void map_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Navigator.MouseLeftButtonDown(sender, e);
        }

        public void map_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            Navigator.MouseWheel(sender, e);
        }

        public void map_MouseMove(object sender, MouseEventArgs e)
        {
            Navigator.MouseMove(sender, e);
        }

        public void SelectMasterFolder_Click(object sender, EventArgs e)
        {
            SelectMap.SelectMainFolder(sender, e);
        }

        public void OpenFileFromList(object sender, RoutedEventArgs e)
        {
            Process fileopener = new(); //Start a new process under the variable of fileopener
            fileopener.StartInfo.FileName = "explorer"; //Open the windows explorer/files; no other program
            fileopener.StartInfo.Arguments = SelectedItem.FilePath; //Open the file with respective filepath
            fileopener.Start(); //Open file
        }
        
        public static explicit operator MainWindow(WindowCollection v)
        {
            throw new NotImplementedException();
        }

        public void GameSelected(object sender, RoutedEventArgs e)
        {
            if (GameSelect.SelectedItem.ToString().Contains("Victoria II"))
            {
                GameSoundHandler.SoundHandler.Gamemode = "VIC2";
            }
            else if (GameSelect.SelectedItem.ToString().Contains("Europa Universalis IV"))
            {
                GameSoundHandler.SoundHandler.Gamemode = "EU4";
            }
            GameSoundHandler.SoundHandler.PlayConnectingSound();
        }
    }
}
