using Paradox_Editor.A_Map_Functions;
using Paradox_Editor.C_Window_Functions;
using Paradox_Editor.D_Types;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using WpfAnimatedGif;

namespace Paradox_Editor
{
    [ToolboxItem(true)]

    /// <summary>
    /// Interaction logic for ProvinceInterface.xaml
    /// </summary>
    public partial class ProvinceInterface : UserControl
    {
        private DataAcquisition ModData { get; set; }
        private int InterfaceActualRows;

        //For usage in Data Binding
        public ObservableCollection<Core> Cores { get; set; } = new ObservableCollection<Core>();
        public ObservableCollection<StateBuilding> StateBuildings { get; set; } = new ObservableCollection<StateBuilding>();

        /*        static ProvinceInterface()
                {
                    DefaultStyleKeyProperty.OverrideMetadata(typeof(ProvinceInterface),
                        new FrameworkPropertyMetadata(typeof(ProvinceInterface)));
                }*/
        public ProvinceInterface() { InitializeComponent(); }
        public ProvinceInterface(DataAcquisition modData)
        {
            InitializeComponent();
            ModData = modData;
            Set_Save_Icon_To_Saved();
        }

        public void PopulateInterface()
        {

        }

        public void Update()
        {
            background.Source = VisualHandler.InterfaceAssetSet.Interface_Background;
            AddCore_Image.Source = VisualHandler.InterfaceAssetSet.Add_Icon;
            ResetCore_Image.Source = VisualHandler.InterfaceAssetSet.Reset_Icon;
            ExitButton_Image.Source = VisualHandler.InterfaceAssetSet.Exit_Icon;

            PROVIDBOX.Foreground = VisualHandler.InterfaceAssetSet.TextColor;
            NAMEBOX.Foreground = VisualHandler.InterfaceAssetSet.TextColor;
            Color.Foreground = VisualHandler.InterfaceAssetSet.TextColor;
            COLORRGB.Foreground = VisualHandler.InterfaceAssetSet.TextColor;
            Owner.Foreground = VisualHandler.InterfaceAssetSet.TextColor;
            OWNERBOX.Foreground = VisualHandler.InterfaceAssetSet.TextColor;
            Controller.Foreground = VisualHandler.InterfaceAssetSet.TextColor;
            CONTROLLERBOX.Foreground = VisualHandler.InterfaceAssetSet.TextColor;
            TradeGood.Foreground = VisualHandler.InterfaceAssetSet.TextColor;
            TRADEGOODBOX.Foreground = VisualHandler.InterfaceAssetSet.TextColor;
            LifeRating.Foreground = VisualHandler.InterfaceAssetSet.TextColor;
            LIFERATINGBOX.Foreground = VisualHandler.InterfaceAssetSet.TextColor;
            Colonial.Foreground = VisualHandler.InterfaceAssetSet.TextColor;
            COLONIALBOX.Foreground = VisualHandler.InterfaceAssetSet.TextColor;
            CoresText.Foreground = VisualHandler.InterfaceAssetSet.TextColor;
        }

        public void CloseInterface(object sender, EventArgs e)
        {
            Visibility = Visibility.Hidden;
            SoundHandler.PlayClickSound();
        }

        public void Save_Button_Pressed(object sender, RoutedEventArgs e)
        {
            //Creates TEMPORARY test file
            var fileName = "Test.txt";
            var path = @"C:\Program Files (x86)\Steam\steamapps\common\Victoria 2\mod\TestPrimaryEnvironment(DoD)\output\" + fileName;

            if (File.Exists(path))
                File.Delete(path);

            File.CreateText(path);
            using (StreamReader sr = File.OpenText(path)) // Open file
            {
                string s = "";
                while ((s = sr.ReadLine()) != null)
                    Debug.WriteLine(s);
            }

            Set_Save_Icon_To_Saved();
        }

        public void Interface_Changed(object sender, RoutedEventArgs e)
        {
            //---------Changes the Save Icon to Warning Gif----------------
            var newGif = new BitmapImage(new Uri(@"/Preloaded_Assets/save_warning_button.gif", UriKind.Relative));
            ImageBehavior.SetAnimatedSource(Save_Button, newGif);
            //-------------------------------------------------------------
        }

        /// <summary>
        /// Changes the Save Icon to Confirmed Gif
        /// </summary>
        public void Set_Save_Icon_To_Saved()
        {
            var newGif = new BitmapImage(new Uri(@"/Preloaded_Assets/save_confirm_button.gif", UriKind.Relative));
            ImageBehavior.SetAnimatedSource(Save_Button, newGif);
        }




        public void ResetCores(object sender, RoutedEventArgs e)
        {
            var rowCount = ProvinceInterfaceViewerGrid.RowDefinitions.Count;
            if (rowCount > InterfaceActualRows + 1) //Requires changing every time the row count is altered.
            {
                ProvinceInterfaceViewerGrid.RowDefinitions.RemoveAt(rowCount - 1);
                var heightChange = COREGRID.Height - 25;
                COREGRID.Height = heightChange;
                CoreGridRow.Height = new GridLength(heightChange);
            }
            else if (rowCount == InterfaceActualRows)
            {
                COREGRID.Height = 0;
            }
            else
            {
                ProvinceInterfaceViewerGrid.RowDefinitions.RemoveAt(rowCount - 1);
                COREGRID.Height = 0;
            }
            Interface_Changed(sender, e); //Notify data has been changed.
        }

        public void ResetListRows(object sender, RoutedEventArgs e)
        {
            var rowCount = ProvinceInterfaceViewerGrid.RowDefinitions.Count;
            for (int i = rowCount; i > InterfaceActualRows; i--)
            {
                ProvinceInterfaceViewerGrid.RowDefinitions.RemoveAt(rowCount - 1);
                rowCount = ProvinceInterfaceViewerGrid.RowDefinitions.Count;
            }
            COREGRID.Height = 0;
            CoreGridRow.Height = new GridLength(25);
            STATEBUILDING_GRID.Height = 0;
            StateBuildingGridRow.Height = new GridLength(44);

            Interface_Changed(sender, e); //Notify data has been changed.
        }

        public void AddBlankCore(object sender, RoutedEventArgs e)
        {
            Cores.Add(new Core("")); //Adds the Blank Core
            //InterfaceHandler.AddInterfaceRow();
            Interface_Changed(sender, e); //Notify data has been changed.
        }

        public void RemoveCore(object sender, RoutedEventArgs e)
        {
            Cores.RemoveAt(COREGRID.SelectedIndex); //Removes Core
            //InterfaceHandler.RemoveInterfaceRow();
            Interface_Changed(sender, e); //Notify data has been changed.
        }

    }
}

