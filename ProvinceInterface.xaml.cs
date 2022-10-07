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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WpfAnimatedGif;

namespace Paradox_Editor
{
    [ToolboxItem(true)]
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
        public ProvinceInterface()
        {
            InitializeComponent();
            Set_Save_Icon_To_Saved();
        }

        public void SetModData(DataAcquisition modData)
        {
            ModData = modData;
        }

        public void PopulateInterface()
        {

        }

        public void UpdateUI()
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

        #region //REWORKING THESE METHODS
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
        #endregion


        static uint GetRawColor(Color color) => (0xFFu << 24)
            | ((uint)color.R << 16) | ((uint)color.G << 8) | ((uint)color.B);
        //Adjust for proper data extraction
        public void SetInterfaceData(Color color) //Crashes if the province has no owner
        {

            ModData.GetColorsToProvinceIDs().TryGetValue(GetRawColor(color), out var provinceID);
            ModData.GetProvinces().TryGetValue((uint)provinceID, out var province);
            if (province != null)
            {
                PROVIDBOX.Text = Convert.ToString(province.ProvinceID);
                NAMEBOX.Text = Convert.ToString(province.ProvinceName);
                OWNERBOX.Text = Convert.ToString(province.Owner);
                CONTROLLERBOX.Text = Convert.ToString(province.Controller);
                COLORRGB.Text = Convert.ToString(color.R + "," + color.G + "," + color.B);
                TRADEGOODBOX.Text = Convert.ToString(province.TradeGoods);
                LIFERATINGBOX.Text = Convert.ToString(province.LifeRating);
                COLONIALBOX.Text = Convert.ToString(province.Colonial);
                NAVALBASEBOX.Text = Convert.ToString(province.Naval_Base);
                TERRAINBOX.Text = Convert.ToString(province.Terrain);
                FORTBOX.Text = Convert.ToString(province.Fort);
                RAILROADBOX.Text = Convert.ToString(province.Railroad);

                ///Below code is still under development
                foreach (var core in province.Cores)
                {
                    Cores.Add(new Core(core));
                }
                foreach (var stateBuilding in province.State_Buildings)
                {
                    StateBuildings.Add(stateBuilding);
                }

                this.Visibility = Visibility.Visible;
            }
            else
            {
                Debug.WriteLine("Ocean/Water province clicked. Hiding interface! ");
                this.Visibility = Visibility.Hidden;
            }







        }

    }
}

