using Paradox_Editor.Extensions;
using Paradox_Editor.Handlers;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using Paradox_Editor.Interfaces;
using Paradox_Editor.Types;
using Paradox_Editor.Types.Data;

//F1 to see WIKI detail on part
//F12 to see usage in VS
//CTRL +press+ K, D sorts all tabs

namespace Paradox_Editor;

public partial class MainWindow : INotifyPropertyChanged
{
    /// Acquires the data under ProvinceFile; ID, provinceName, Filepath
    public ProvinceWrapper SelectedItem { get; set; }

    public int CurrentControlMode { get; set; }
    public static string CurrentGameMode { get; set; } = null!;

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void NotifyPropertyChange(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private ObservableCollection<ProvinceWrapper> _provinceData = [];

    public ObservableCollection<ProvinceWrapper> ProvinceData
    {
        get => _provinceData;
        set
        {
            _provinceData = value;
            NotifyPropertyChange(nameof(ProvinceData));
        }
    }

    public MainWindow()
    {
        InitializeComponent();
        DataContext = this;
    }

    private void MainWindow_Load(object _1, EventArgs _2)
    {
        VisualHandler.ConductAssetChange("VIC2");
        mapModeButtons.UpdateMapModeVisibility(MapViewer.MapMode.Provincial, VisualHandler.MapModeIconSet);
    }

    private ModData _modData;

    public void SelectMasterFolder_Click(object sender, EventArgs e)
    {
        //var MainWindow = (MainWindow)Application.Current.MainWindow; //May be useless

        string selectedModFolder = Explorer.OpenFolderSelect();
        if (string.IsNullOrEmpty(selectedModFolder))
            return;

        _modData = new ModData(selectedModFolder);

        // For the History File Lister. Ignores ocean provinces.
        ProvinceData = new ObservableCollection<ProvinceWrapper>(_modData.DatabaseProvinces.GetLandProvinceWrappers());
        
        fileListView.ItemsSource = ProvinceData;

        mapViewer.LoadMaps();
        popAdjuster.Update();
        provinceInterface.Update();
    }

    public void OpenFileFromList(object sender, RoutedEventArgs e)
    {
        Explorer.OpenFile(SelectedItem.File);
    }

    public static explicit operator MainWindow(WindowCollection v) => throw new NotImplementedException();

    public void SelectGame(object sender, RoutedEventArgs e)
    {
        var game = GameSelectDropdown.SelectedItem.ToString();
        switch (game)
        {
            case not null when game.Contains("Victoria II"):
                SoundHandler.SoundAssetChange("VIC2");
                VisualHandler.ConductAssetChange("VIC2");
                break;
            case not null when game.Contains("Europa Universalis IV"):
                SoundHandler.SoundAssetChange("EU4");
                VisualHandler.ConductAssetChange("EU4");
                break;
            default:
                SoundHandler.SoundAssetChange("VIC2");
                VisualHandler.ConductAssetChange("VIC2");
                break;
        }

        provinceInterface.UpdateUI();
        mapModeButtons.UpdateMapModeVisibility(mapModeButtons.GetMapMode(), VisualHandler.MapModeIconSet);
        SoundHandler.PlayConnecting();
    }

    private void ChangeControlScheme(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        => CurrentControlMode = ControlMode.SelectedIndex;

    // Hide the province interface on boot.
    private void provinceInterface_Loaded(object sender, RoutedEventArgs e)
        => provinceInterface.Visibility = Visibility.Hidden;
}