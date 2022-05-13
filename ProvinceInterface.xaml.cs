using Paradox_Editor.B_Data_Functions;
using Paradox_Editor.C_Window_Functions;
using Paradox_Editor.D_Types;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WpfAnimatedGif;

namespace Paradox_Editor
{

    public partial class FileInterface : UserControl
    {
        public ObservableCollection<CoreData> CoreDataCollection { get; set; } = new ObservableCollection<CoreData>();
        public ObservableCollection<StateBuilding> StateBuildingCollection { get; set; } = new ObservableCollection<StateBuilding>();
        public InterfaceRowHandler InterfaceHandler { get; set; } = new InterfaceRowHandler();
        public MainWindow MainWindow {get; set;} = (MainWindow)Application.Current.MainWindow;

        public FileInterface()
        {
            InterfaceHandler.MainWindow = MainWindow;
            InitializeComponent();
            Set_Save_Icon_To_Saved();
        }

        public void ExitClicked(object sender, EventArgs e)
        {
            Visibility = Visibility.Hidden;
            GameSoundHandler.SoundHandler.PlayClickSound();
        }

        public void RemoveCoreRow(object sender, RoutedEventArgs e)
        {
            CoreDataCollection.RemoveAt(COREGRID.SelectedIndex); //Removes Core
            InterfaceHandler.RemoveInterfaceRow();
            HistoryFile_Changed(sender, e); //Notify data has been changed.
        }

        public void AddBlankCoreRow(object sender, RoutedEventArgs e)
        {
            CoreData core = new CoreData("");
            CoreDataCollection.Add(core); //Adds the Blank Core
            InterfaceHandler.AddInterfaceRow();
            HistoryFile_Changed(sender, e); //Notify data has been changed.
        }

        public void AddBlankStateBuildingRow(object sender, RoutedEventArgs e)
        {
            var stateBuilding = new StateBuilding("");
            StateBuildingCollection.Add(stateBuilding); //Adds the Blank Core
            InterfaceHandler.AddInterfaceRow();
            HistoryFile_Changed(sender, e); //Notify data has been changed.
        }

        public void ResetCores(object sender, RoutedEventArgs e)
        {
            CoreDataCollection.Clear();
            InterfaceHandler.ResetListRows();

            HistoryFile_Changed(sender, e); //Notify data has been changed.
        }
        
        public void AddCoresAndBuildings(object sender, RoutedEventArgs e, MainWindow MainWindow, System.Drawing.Color PixelColor)
        {
            CoreDataCollection.Clear();
            InterfaceHandler.ResetListRows();
            var boxBinding = new HistoryfileInterface(MainWindow, PixelColor,
                MainWindow.SelectMap.StoredProvinceColorToID,
                MainWindow.SelectMap.StoredProvinceIDToDataDictionaries,
                MainWindow.SelectMap.StoredTagToCountryName);

            var coresList = boxBinding.GetCoreList();
            if (coresList != null) //Oceans don't have cores; they're null
            {
                foreach (var coreEntry in coresList)
                {
                    CoreData core = new CoreData(coreEntry);
                    CoreDataCollection.Add(core);
                    InterfaceHandler.AddInterfaceRow();
                }
            }

            StateBuildingCollection.Clear();
            //InterfaceHandler.ResetStateBuildingListRows();

            var buildingList = boxBinding.GetStateBuildingList();
            if (buildingList != null) //Oceans don't have cores; they're null
            {
                foreach (var buildingEntry in buildingList)
                {
                    var building = new StateBuilding();
                    building.Building = buildingEntry.Building;
                    building.Upgrade = buildingEntry.Upgrade;
                    building.Level = buildingEntry.Level;
                    StateBuildingCollection.Add(building);
                    InterfaceHandler.AddInterfaceRow();
                    ProvinceInterfaceViewerGrid.RowDefinitions.Add(new RowDefinition()); //Adds a Row to Interface

                    var p = MainWindow.FileInterface.STATEBUILDING_GRID.Height + 25;
                    MainWindow.FileInterface.STATEBUILDING_GRID.Height = p;
                    MainWindow.FileInterface.StateBuildingGridRow.Height = new GridLength(p);
                }
            }

        }

         public void HistoryFile_Changed(object sender, RoutedEventArgs e)
        {
            //---------Changes the Save Icon to Warning Gif----------------
            var newGif = new BitmapImage(new Uri(@"/Preloaded_Assets/save_warning_button.gif",UriKind.Relative));
            ImageBehavior.SetAnimatedSource(Save_Button, newGif);
            //-------------------------------------------------------------
        }

        public void Save_Button_Pressed(object sender, RoutedEventArgs e)
        {
            var fileName = "Test.txt";
            var path = @"C:\Program Files (x86)\Steam\steamapps\common\Victoria 2\mod\TestPrimaryEnvironment(DoD)\output\" + fileName;

            var historyFileHandler = new HistoryExportHandler();
            var interfaceEntries = historyFileHandler.GetInterfaceEntries();
            var historyFromInterface = historyFileHandler.ExportEntries(interfaceEntries);

            if (File.Exists(path))
            {
                File.Delete(path);
            }
            using (var streamWriter = File.CreateText(path)) // Create file
            {
                foreach (var entry in historyFromInterface)
                {
                    streamWriter.WriteLine(entry.ToString());
                    Debug.WriteLine(entry);
                }
            }

            using (StreamReader sr = File.OpenText(path)) // Open file
            {
                string s = "";
                while ((s = sr.ReadLine()) != null)
                {
                    Debug.WriteLine(s);
                }
            }

            Set_Save_Icon_To_Saved();
        }

        public void Set_Save_Icon_To_Saved()
        {
            //---------Changes the Save Icon to Confirmed Gif----------------
            var newGif = new BitmapImage(new Uri(@"/Preloaded_Assets/save_confirm_button.gif", UriKind.Relative));
            ImageBehavior.SetAnimatedSource(Save_Button, newGif);
            //-------------------------------------------------------------
        }


    }
}
