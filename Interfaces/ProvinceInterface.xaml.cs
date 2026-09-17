using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Paradox_Editor.Extensions;
using Paradox_Editor.Handlers;
using Paradox_Editor.Types;
using WpfAnimatedGif;
using Color = System.Windows.Media.Color;

namespace Paradox_Editor.Interfaces;

[ToolboxItem(true)]
public partial class ProvinceInterface
{
    private ObservableCollection<Tag> _cores = [];

    //private MainWindow MainWindow { get; set; } = (MainWindow)Application.Current.MainWindow;
    private uint CurrentProvince { get; set; }
    public ObservableCollection<StateBuilding> StateBuildings { get; set; } = [];

    public event PropertyChangedEventHandler PropertyChanged;

    protected void NotifyPropertyChange(string propertyName)
    {
        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
    }

    public ObservableCollection<Tag> Cores
    {
        get => _cores;
        set
        {
            _cores = value;
            NotifyPropertyChange(nameof(Cores));
        }
    }

    public ProvinceInterface()
    {
        InitializeComponent();
        DataContext = this;
        SetSaveIconToSaved();
    }

    public void UpdateUI()
    {
        background.Source = VisualHandler.InterfaceAssetSet.InterfaceBackground;
        AddCore_Image.Source = VisualHandler.InterfaceAssetSet.AddIcon;
        ResetCore_Image.Source = VisualHandler.InterfaceAssetSet.ResetIcon;
        AddBuilding_Icon.Source = VisualHandler.InterfaceAssetSet.AddIcon;
        ResetBuildings_Icon.Source = VisualHandler.InterfaceAssetSet.ResetIcon;
        ExitButton_Image.Source = VisualHandler.InterfaceAssetSet.ExitIcon;

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
        if (ModData.Instance.DatabaseProvinces.GetColor(CurrentProvince) != 0)
        {
            string historyFile = ModData.Instance.DatabaseProvinces.GetHistoryFile(CurrentProvince);
            if (File.Exists(historyFile))
                File.Delete(historyFile);

            StreamWriter file = File.CreateText(historyFile);

            PopulateSaveFile(file);

            //Currently only refreshes political map
            var currentColor = ModData.Instance.DatabaseProvinces.GetIDFromColor(CurrentProvince);
            MapViewer.SetMap(0, new MapRenderer()
                .RefreshProvincePolitical(
                    MapViewer.GetMap(MapViewer.MapMode.PROVENCIAL),
                    MapViewer.GetMap(MapViewer.MapMode.POLITICAL),
                    currentColor)
            );

            SetSaveIconToSaved();
        }
        else
        {
            SoundHandler.PlayError();
            Debug.WriteLine("Province is null. Select a province.");
        }
    }

    private void PopulateSaveFile(StreamWriter file)
    {
        var provinceID = Convert.ToUInt32(PROVIDBOX.Text);
        DatabaseProvinces databaseProvinces = ModData.Instance.DatabaseProvinces;

        bool hasCountry = ModData.Instance.DatabaseCountries.HasCountry(OWNERBOX.Text);
        if (hasCountry)
        {
            file.WriteLine("owner = " + OWNERBOX.Text);
            databaseProvinces.SetOwner(provinceID, OWNERBOX.Text);
        }
        else
        {
            MessageBox.Show("Owner TAG invalid ! : " + OWNERBOX.Text);
            databaseProvinces.SetOwner(provinceID);
        }

        hasCountry = ModData.Instance.DatabaseCountries.HasCountry(CONTROLLERBOX.Text);
        if (hasCountry)
        {
            file.WriteLine("controller = " + CONTROLLERBOX.Text);
            databaseProvinces.SetController(provinceID, CONTROLLERBOX.Text);
        }
        else
        {
            MessageBox.Show("Controller TAG invalid! : " + CONTROLLERBOX.Text);
            databaseProvinces.SetController(provinceID);
        }

        if (TRADEGOODBOX.Items.GetItemAt(TRADEGOODBOX.SelectedIndex) is ComboBox tradeGoodDropdown &&
            tradeGoodDropdown.SelectedIndex != 0)
        {
            string goodText = tradeGoodDropdown.Text;
            file.WriteLine("trade_goods = " + goodText);
            databaseProvinces.SetTradeGood(provinceID, goodText);
        }

        if (!LIFERATINGBOX.Text.Equals("") && !LIFERATINGBOX.Text.Equals("0"))
        {
            file.WriteLine("life_rating = " + LIFERATINGBOX.Text + "\t");
            databaseProvinces.SetLifeRating(provinceID, Convert.ToInt16(LIFERATINGBOX.Text));
        }

        if (!COLONIALBOX.Text.Equals(""))
        {
            file.WriteLine("colonial = " + COLONIALBOX.Text + "\t");
            databaseProvinces.SetColonial(provinceID, Convert.ToInt16(COLONIALBOX.Text));
        }

        if (COREGRID.HasItems)
        {
            databaseProvinces.ClearCores(provinceID);
            foreach (Tag item in COREGRID.Items)
            {
                if (string.IsNullOrEmpty(item.Value))
                    continue;
                file.WriteLine("add_core = " + item.Value);
                databaseProvinces.AddCore(provinceID, item);
            }
        }
        else
        {
            databaseProvinces.ClearCores(provinceID);
        }

        // TODO: Simplify this area.
        if (!TERRAINBOX.Text.Equals(""))
        {
            file.WriteLine("terrain = " + TERRAINBOX.Text);
            databaseProvinces.SetTerrain(provinceID, TERRAINBOX.Text);
        }

        if (!NAVALBASEBOX.Text.Equals("") && !NAVALBASEBOX.Text.Equals("0"))
        {
            file.WriteLine("naval_base = " + NAVALBASEBOX.Text + "\t");
            databaseProvinces.SetNavalBaseLevel(provinceID, Convert.ToInt16(NAVALBASEBOX.Text));
        }

        if (!FORTBOX.Text.Equals("") && !FORTBOX.Text.Equals("\t"))
        {
            file.WriteLine("fort = " + FORTBOX.Text + "\t");
            databaseProvinces.SetFortLevel(provinceID, Convert.ToInt16(FORTBOX.Text));
        }

        if (!RAILROADBOX.Text.Equals("") && !RAILROADBOX.Text.Equals("0"))
        {
            file.WriteLine("railroad = " + RAILROADBOX.Text + "\t");
            databaseProvinces.SetRailroadLevel(provinceID, Convert.ToInt16(RAILROADBOX.Text));
        }

        databaseProvinces.ClearBuildings(provinceID);
        if (STATEBUILDING_GRID.HasItems)
        {
            foreach (StateBuilding building in STATEBUILDING_GRID.Items)
            {
                if (string.IsNullOrEmpty(building.Building) &&
                    string.IsNullOrEmpty(building.Level) &&
                    string.IsNullOrEmpty(building.Upgrade))
                {
                    file.WriteLine("state_building = {");
                    file.WriteLine("\tlevel = " + building.Level);
                    file.WriteLine("\tbuilding = " + building.Building);
                    file.WriteLine("\tupgrade = " + building.Upgrade);
                    file.WriteLine("}");
                }

                databaseProvinces.AddStateBuilding(provinceID, building);
            }
        }

        file.Close();
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
    public void SetSaveIconToSaved()
    {
        var newGif = new BitmapImage(new Uri(@"/Assets/save_confirm_button.gif", UriKind.Relative));
        ImageBehavior.SetAnimatedSource(Save_Button, newGif);
    }

    #region Core and State-Building Button Events

    public void ResetCores(object sender, RoutedEventArgs e)
    {
        Cores.Clear();
        Interface_Changed(sender, e);
    }

    public void AddBlankCore(object sender, RoutedEventArgs e)
    {
        Cores.Add(new Tag(""));
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

    /// <summary>
    /// Extrapolates province data from a given colour and fills the province interface with the said-data.
    /// </summary>
    /// <param name="color"></param>
    public void PopulateInterface(Color color)
    {
        DatabaseProvinces databaseProvinces = ModData.Instance.DatabaseProvinces;
        var provinceID = databaseProvinces.GetIDFromColor(color);
        CurrentProvince = provinceID;
        if (!databaseProvinces.IsValidLandProvince(provinceID))
        {
            PROVIDBOX.Text = Convert.ToString(provinceID);

            // Set name.
            NAMEBOX.Text = databaseProvinces.GetName(provinceID);

            // Set owner.
            databaseProvinces.TryGetOwner(provinceID, out var owner);
            OWNERBOX.Text = owner ?? "";

            // Set controller.
            databaseProvinces.TryGetController(provinceID, out var controller);
            CONTROLLERBOX.Text = controller ?? "";

            // Set Color display.
            COLORRGB.Text = Convert.ToString(color.R + "," + color.G + "," + color.B);

            string loggedTradeGood = databaseProvinces.GetTradeGood(provinceID);
            bool breakLoop = false;
            for (int i = 1; i < TRADEGOODBOX.Items.Count; i++) //"Foreach TradeGood Group"
            {
                for (int k = 1; k < TRADEGOODBOX.Items.Count; k++)
                {
                    // Wipe selections.
                    (TRADEGOODBOX.Items.GetItemAt(k) as ComboBox)!.SelectedIndex = 0;
                }

                if (TRADEGOODBOX.Items.GetItemAt(i) is ComboBox goodGroup)
                    for (int j = 1; j < goodGroup.Items.Count; j++) //"Foreach Good in Group"
                    {
                        var goodGroupItem = goodGroup.Items[j];
                        if (goodGroupItem != null && !goodGroupItem.Equals(loggedTradeGood))
                            continue;

                        TRADEGOODBOX.SelectedIndex = i;
                        (TRADEGOODBOX.Items.GetItemAt(i) as ComboBox)!.SelectedIndex = j;
                        breakLoop = true;
                        break;
                    }

                if (breakLoop)
                    break;
            }

            LIFERATINGBOX.Text = Convert.ToString(databaseProvinces.GetLifeRating(provinceID));
            COLONIALBOX.Text = Convert.ToString(databaseProvinces.GetColonial(provinceID));
            NAVALBASEBOX.Text = Convert.ToString(databaseProvinces.GetNavalBaseLevel(provinceID));
            TERRAINBOX.Text = Convert.ToString(databaseProvinces.GetTerrain(provinceID));
            FORTBOX.Text = Convert.ToString(databaseProvinces.GetFortLevel(provinceID));
            RAILROADBOX.Text = Convert.ToString(databaseProvinces.GetRailroadLevel(provinceID));

            object sender = this;
            RoutedEventArgs e = new();

            ResetCores(sender, e);
            foreach (Tag core in databaseProvinces.GetCores(provinceID)) //Populates Core List
                Cores.Add(core);
            ResetBuildings(sender, e);
            foreach (StateBuilding stateBuilding in databaseProvinces.GetStateBuildings(provinceID))
                StateBuildings.Add(stateBuilding); // WARN: Does not work!

            Visibility = Visibility.Visible;
        }
        else
        {
            Debug.WriteLine("Ocean/Water province clicked. Hiding interface! ");
            Visibility = Visibility.Hidden;
        }

        SetSaveIconToSaved();
    }

    private void Interface_Loaded(object sender, RoutedEventArgs e)
    {
        var window = Window.GetWindow(this);
        if (window != null)
            window.KeyDown += HandleKeyPress;
    }

    private void HandleKeyPress(object sender, KeyEventArgs e)
    {
        if ((Keyboard.Modifiers & ModifierKeys.Control) != ModifierKeys.Control)
            return;
        if (Keyboard.IsKeyDown(Key.S))
            Save();
    }

    internal void Update()
    {
        for (int i = 1; i < TRADEGOODBOX.Items.Count; i++)
            TRADEGOODBOX.Items.RemoveAt(i);

        foreach (var group_of_goods in ModData.Instance.COMMON_DATA.Goods)
        {
            // Populate goods groups.
            var groupComboBox = new ComboBox { Name = group_of_goods.Key };
            var good_item = new ComboBoxItem() //Blank Option
            {
                Content = group_of_goods.Key, Focusable = false, IsHitTestVisible = false, IsSelected = true,
                HorizontalContentAlignment = HorizontalAlignment.Center, FontWeight = FontWeights.Bold,
            };

            // Populate with even more goods.
            groupComboBox.Items.Add(good_item);
            foreach (var good in group_of_goods.Value.Goods.Keys)
            {
                groupComboBox.Items.Add(good);
            }

            TRADEGOODBOX.Items.Add(groupComboBox);
        }
    }
}