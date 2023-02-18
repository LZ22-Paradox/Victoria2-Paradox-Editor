using Paradox_Editor.Data_Handling;
using Paradox_Editor.Extensions;
using Paradox_Editor.Extensions.Types;
using Paradox_Editor.Handlers;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WpfAnimatedGif;

namespace Paradox_Editor
{
    [ToolboxItem(true)]
    public partial class ProvinceInterface : UserControl
    {
        
        private MainWindow MainWindow { get; set; } = (MainWindow)Application.Current.MainWindow;
        private ProvinceFile _currentProvince = new();
        private ProvinceFile CurrentProvince { get { return _currentProvince; } set { _currentProvince = value; } }

        public ObservableCollection<StateBuilding> StateBuildings { get; set; } = new ObservableCollection<StateBuilding>();

        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChange(string propertyName)
        { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); }
        //Crashes after loading second mod
        private ObservableCollection<Core> _cores = new();
        public ObservableCollection<Core> Cores
        { get => _cores; set { _cores = value; NotifyPropertyChange(nameof(Cores)); } }
        public ProvinceInterface()
        {
            InitializeComponent();
            this.DataContext = this;
            Set_Save_Icon_To_Saved();
        }

        public void UpdateUI()
        {
            background.Source = VisualHandler.InterfaceAssetSet.Interface_Background;
            AddCore_Image.Source = VisualHandler.InterfaceAssetSet.Add_Icon;
            ResetCore_Image.Source = VisualHandler.InterfaceAssetSet.Reset_Icon;
            AddBuilding_Icon.Source = VisualHandler.InterfaceAssetSet.Add_Icon;
            ResetBuildings_Icon.Source = VisualHandler.InterfaceAssetSet.Reset_Icon;
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
            Terrain.Foreground = VisualHandler.InterfaceAssetSet.TextColor;
            TERRAINBOX.Foreground = VisualHandler.InterfaceAssetSet.TextColor;
            Naval_Base.Foreground = VisualHandler.InterfaceAssetSet.TextColor;
            NAVALBASEBOX.Foreground = VisualHandler.InterfaceAssetSet.TextColor;
            Fort.Foreground = VisualHandler.InterfaceAssetSet.TextColor;
            FORTBOX.Foreground = VisualHandler.InterfaceAssetSet.TextColor;
            Railroad.Foreground = VisualHandler.InterfaceAssetSet.TextColor;
            RAILROADBOX.Foreground = VisualHandler.InterfaceAssetSet.TextColor;
            State_Buildings.Foreground = VisualHandler.InterfaceAssetSet.TextColor;
            CoresText.Foreground = VisualHandler.InterfaceAssetSet.TextColor;
        }

        public void CloseInterface(object sender, EventArgs e)
        {
            Visibility = Visibility.Hidden;
            SoundHandler.PlayClick();
        }

        public void SaveButtonPressed(object sender, RoutedEventArgs e) => Save();

        private void Save()
        {
            if (CurrentProvince.color != 0)
            {
                if (File.Exists(CurrentProvince.HistoryFilePath))
                    File.Delete(CurrentProvince.HistoryFilePath);

                var file = File.CreateText(CurrentProvince.HistoryFilePath);

                PopulateSaveFile(file);

                //vCurrently only refreshes political map
                MapViewer.SetMap(0, new MapRenderer(ModData.PROVINCE_DATA)
                    .RefreshProvincePolitical(MapViewer.GetMap(1), MapViewer.GetMap(0), CurrentProvince.color));

                Set_Save_Icon_To_Saved();
            }
            else
            {
                SoundHandler.PlayError();
                Debug.WriteLine("Province is null. Select a province.");
            }
        }

        void PopulateSaveFile(StreamWriter file)
        {
            ProvinceFile tempFile = ModData.PROVINCE_DATA.GetProvince(Convert.ToUInt32(PROVIDBOX.Text));
            if (!OWNERBOX.Text.Equals(""))
            {
                ModData.COUNTRY_DATA.GetCountries().TryGetValue(OWNERBOX.Text, out var country);
                if (country != null)
                {
                    file.WriteLine("owner = " + OWNERBOX.Text);
                    tempFile.Owner = OWNERBOX.Text;
                }
                else
                {
                    MessageBox.Show("Owner TAG invalid! : " + OWNERBOX.Text);
                    tempFile.Owner = null;
                }
            }
            else
            {
                tempFile.Owner = null;
            }
            if (!CONTROLLERBOX.Text.Equals(""))
            {
                ModData.COUNTRY_DATA.GetCountries().TryGetValue(CONTROLLERBOX.Text, out var country);
                if (country != null)
                {
                    file.WriteLine("controller = " + CONTROLLERBOX.Text);
                    tempFile.Controller = CONTROLLERBOX.Text;
                }
                else
                {
                    MessageBox.Show("Controller TAG invalid! : " + CONTROLLERBOX.Text);
                    tempFile.Controller = null;
                }
            }
            else
            {
                tempFile.Controller = null;
            }
            if (!TRADEGOODBOX.Text.Equals(""))
            {
                file.WriteLine("trade_goods = " + TRADEGOODBOX.Text);
                tempFile.TradeGoods = TRADEGOODBOX.Text;
            }
            if (!LIFERATINGBOX.Text.Equals("") && !LIFERATINGBOX.Text.Equals("0"))
            {
                file.WriteLine("life_rating = " + LIFERATINGBOX.Text + "\t");
                tempFile.LifeRating = Convert.ToInt16(LIFERATINGBOX.Text);
            }
            if (!COLONIALBOX.Text.Equals("") && !COLONIALBOX.Text.Equals("0"))
            {
                file.WriteLine("colonial = " + COLONIALBOX.Text + "\t");
                tempFile.Colonial = Convert.ToInt16(COLONIALBOX.Text);
            }
            if (COREGRID.HasItems)
            {
                tempFile.Cores.Clear();
                foreach (Core item in COREGRID.Items)
                {
                    if (item.TAG is not "" or null)
                    {
                        file.WriteLine("add_core = " + item.TAG);
                        tempFile.Cores.Add(item);
                    }
                }
            } else
            {
                tempFile.Cores.Clear();
            }
            if (!TERRAINBOX.Text.Equals(""))
            {
                file.WriteLine("terrain = " + TERRAINBOX.Text);
                tempFile.Terrain = TERRAINBOX.Text;
            }
            if (!NAVALBASEBOX.Text.Equals("") && !NAVALBASEBOX.Text.Equals("0"))
            {
                file.WriteLine("naval_base = " + NAVALBASEBOX.Text  + "\t");
                tempFile.Naval_Base = Convert.ToInt16(NAVALBASEBOX.Text);
            }
            if (!FORTBOX.Text.Equals("") && !FORTBOX.Text.Equals("\t"))
            {
                file.WriteLine("fort = " + FORTBOX.Text + "\t");
                tempFile.Fort = Convert.ToInt16(FORTBOX.Text);
            }
            if (!RAILROADBOX.Text.Equals("") && !RAILROADBOX.Text.Equals("0"))
            {
                file.WriteLine("railroad = " + RAILROADBOX.Text + "\t");
                tempFile.Railroad = Convert.ToInt16(RAILROADBOX.Text);
            }
            if (STATEBUILDING_GRID.HasItems)
            {
                tempFile.State_Buildings.Clear();
                foreach (StateBuilding building in STATEBUILDING_GRID.Items)
                {
                    if (building.Building != null && building.Level != null && building.Upgrade != null)
                    {
                        file.WriteLine("state_building = {");
                        file.WriteLine("\tlevel = " + building.Level);
                        file.WriteLine("\tbuilding = " + building.Building);
                        file.WriteLine("\tupgrade = " + building.Upgrade);
                        file.WriteLine("}");
                    }
                    tempFile.State_Buildings.Add(building);
                }
            }

            file.Close();

            ModData.PROVINCE_DATA.ReplaceProvince(tempFile);
            SoundHandler.PlayConnecting(); //Perhaps change to success sound or change connecting sound to something else?
        }

        /// <summary>
        /// Changes the Save Icon to Warning Gif
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Interface_Changed(object sender, RoutedEventArgs e)
        {
            var newGif = new BitmapImage(new Uri(@"/Assets/save_warning_button.gif", UriKind.Relative));
            ImageBehavior.SetAnimatedSource(Save_Button, newGif);
        }

        /// <summary>
        /// Changes the Save Icon to Confirmed Gif
        /// </summary>
        public void Set_Save_Icon_To_Saved()
        {
            var newGif = new BitmapImage(new Uri(@"/Assets/save_confirm_button.gif", UriKind.Relative));
            ImageBehavior.SetAnimatedSource(Save_Button, newGif);
        }

        #region Custom Core and State-Building Handling
        public void ResetCores(object sender, RoutedEventArgs e)
        {
            Cores.Clear();
            Interface_Changed(sender, e);
        }
        public void AddBlankCore(object sender, RoutedEventArgs e)
        {
            Cores.Add(new Core(""));
            Interface_Changed(sender, e);
        }
        public void RemoveCore(object sender, RoutedEventArgs e)
        {
            Cores.RemoveAt(COREGRID.SelectedIndex);
            Interface_Changed(sender, e);
        }

        public void ResetBuildings(object sender, RoutedEventArgs e)
        {
            StateBuildings.Clear();
            Interface_Changed(sender, e);
        }
        public void AddBlankBuilding(object sender, RoutedEventArgs e)
        {
            StateBuildings.Add(new StateBuilding());
            Interface_Changed(sender, e);
        }
        public void RemoveBuilding(object sender, RoutedEventArgs e)
        {
            StateBuildings.RemoveAt(STATEBUILDING_GRID.SelectedIndex);
            Interface_Changed(sender, e);
        }
        #endregion


        static uint GetRawColor(Color color) => (0xFFu << 24) | ((uint)color.R << 16) | ((uint)color.G << 8) | ((uint)color.B);
        /// <summary>
        /// Extrapolates province data from a given colour and fills the province interface with the said-data.
        /// </summary>
        /// <param name="color"></param>
        public void PopulateInterface(Color color)
        {
            ModData.PROVINCE_DATA.GetColorsToProvinceIDs().TryGetValue(GetRawColor(color), out var provinceID);
            ProvinceFile province = ModData.PROVINCE_DATA.GetProvince((uint)provinceID);
            CurrentProvince = province;
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

                object sender = this; RoutedEventArgs e = new();

                ResetCores(sender, e);
                foreach (var core in province.Cores) //Populates Core List
                    Cores.Add(core);
                ResetBuildings(sender, e);
                foreach (var stateBuilding in province.State_Buildings)
                    StateBuildings.Add(stateBuilding); //NonFunctional, see "StateBuildings" binding

                this.Visibility = Visibility.Visible;
            }
            else
            {
                Debug.WriteLine("Ocean/Water province clicked. Hiding interface! ");
                this.Visibility = Visibility.Hidden;
            }
            Set_Save_Icon_To_Saved();
        }

        private void Interface_Loaded(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            window.KeyDown += HandleKeyPress;
        }
        private void HandleKeyPress(object sender, KeyEventArgs e)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control) // Is Alt key pressed
                if (Keyboard.IsKeyDown(Key.S))
                    Save();
        }

        //D_

    }
}

