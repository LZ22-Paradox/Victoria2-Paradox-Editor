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
        public InterfaceRowHandler InterfaceHandler { get; set; } = new InterfaceRowHandler();
        public MainWindow MainWindow {get; set;} = (MainWindow)Application.Current.MainWindow;
        private Point start;

        public FileInterface()
        {
            InterfaceHandler.MainWindow = MainWindow;
            this.MouseLeftButtonDown += new MouseButtonEventHandler(LeftButtonDown);
            this.MouseLeftButtonUp += new MouseButtonEventHandler(LeftButtonUp);
            this.MouseMove += new MouseEventHandler(Grid_MouseMove);

            //

            InitializeComponent();


            Set_Save_Icon_To_Saved();
        }

        public void ExitClicked(object sender, EventArgs e)
        {
            Visibility = Visibility.Hidden;
            GameSoundHandler.SoundHandler.PlayClickSound();
        }

        public void LeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ReleaseMouse();
        }
        private void ReleaseMouse()
        {
            this.ReleaseMouseCapture();
            this.Cursor = Cursors.Arrow;
        }

        public void LeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.IsMouseCaptured) return;
            this.Cursor = Cursors.ScrollAll;
            start = e.MouseDevice.GetPosition(this);
            this.CaptureMouse();
        }

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            if (!this.IsMouseCaptured) return;
            var end = e.MouseDevice.GetPosition(this);
            var m = this.RenderTransform.Value;
            m.OffsetX -= start.X - end.X;
            m.OffsetY -= start.Y - end.Y;
            this.RenderTransform = new MatrixTransform(m); //See VisualOffset for future alterations
        }

        public void RemoveCoreRow(object sender, RoutedEventArgs e)
        {
            CoreDataCollection.RemoveAt(COREGRID.SelectedIndex); //Removes Core
            InterfaceHandler.RemoveRowFromCoreList();

            HistoryFile_Changed(sender, e); //Notify data has been changed.
        }

        public void AddCoreRow(object sender, RoutedEventArgs e)
        {

            CoreData core = new CoreData("");
            CoreDataCollection.Add(core); //Adds the Blank Core

            InterfaceHandler.AddRowFromCoreList();

            HistoryFile_Changed(sender, e); //Notify data has been changed.
        }

        public void ResetCores(object sender, RoutedEventArgs e)
        {
            CoreDataCollection.Clear();
            InterfaceHandler.ResetRowsFromCoreList();

            HistoryFile_Changed(sender, e); //Notify data has been changed.
        }

        public void AddExistingCores(object sender, RoutedEventArgs e, MainWindow MainWindow, System.Drawing.Color PixelColor)
        {
            CoreDataCollection.Clear();
            InterfaceHandler.ResetRowsFromCoreList();
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

                    InterfaceHandler.AddRowFromCoreList();
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
