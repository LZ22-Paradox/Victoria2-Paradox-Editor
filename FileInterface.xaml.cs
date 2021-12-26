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
        public static InterfaceRowHandler InterfaceHandler { get; set; } = new InterfaceRowHandler();


        private Point start;

        public FileInterface()
        {
            InterfaceHandler.MainWindow = (MainWindow)Application.Current.MainWindow;

            this.MouseLeftButtonDown += new MouseButtonEventHandler(LeftButtonDown);
            this.MouseLeftButtonUp += new MouseButtonEventHandler(LeftButtonUp);
            this.MouseMove += new MouseEventHandler(Grid_MouseMove);

            //

            InitializeComponent();
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

            //---------Changes the Save Icon to Warning Gif----------------
            var newGif = new BitmapImage(new Uri(@"/Preloaded_Assets/save_warning_button.gif", UriKind.Relative));
            ImageBehavior.SetAnimatedSource(Save_Button, newGif);
            //-------------------------------------------------------------
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

        }

        public void AddCoreRow(object sender, RoutedEventArgs e)
        {

            CoreData core = new CoreData("");
            CoreDataCollection.Add(core); //Adds the Blank Core

            InterfaceHandler.AddRowFromCoreList();

        }

        public void ResetCores(object sender, RoutedEventArgs e)
        {
            CoreDataCollection.Clear();
            InterfaceHandler.ResetRowsFromCoreList();
        }

        public void AddExistingCores(object sender, RoutedEventArgs e, MainWindow MainWindow, System.Drawing.Color PixelColor)
        {
            CoreDataCollection.Clear();
            InterfaceHandler.ResetRowsFromCoreList();
            var boxBinding = new InterfaceHistoryfile(MainWindow, PixelColor,
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

    }
}
