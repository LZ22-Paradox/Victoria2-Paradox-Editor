using System;
using System.Windows;
using Paradox_Editor.Extensions.Assets;
using Paradox_Editor.Handlers;

namespace Paradox_Editor.Interfaces;

public partial class MapModesControl
{
    public MapModesControl()
    {
        InitializeComponent();
    }

    private MapViewer.MapMode CurrentMapMode = MapViewer.MapMode.Provincial;

    public MapViewer.MapMode GetMapMode() => CurrentMapMode;

    private void MapmodeButton_Political_Click(object sender, RoutedEventArgs e)
    {
        CurrentMapMode = MapViewer.MapMode.Political;
        UpdateMapModeVisibility(CurrentMapMode, VisualHandler.MapModeIconSet);
        SoundHandler.PlayClick();
    }

    private void MapmodeButton_Provinces_Click(object sender, RoutedEventArgs e)
    {
        CurrentMapMode = MapViewer.MapMode.Provincial;
        UpdateMapModeVisibility(CurrentMapMode, VisualHandler.MapModeIconSet);
        SoundHandler.PlayClick();
    }

    private void MapmodeButton_Terrain_Click(object sender, RoutedEventArgs e)
    {
        CurrentMapMode = MapViewer.MapMode.Terrain;
        UpdateMapModeVisibility(CurrentMapMode, VisualHandler.MapModeIconSet);
        SoundHandler.PlayClick();
    }

    /// Updates and Disables the other existing map modes
    public void UpdateMapModeVisibility(MapViewer.MapMode currentMapMode, MapMode_IconSet assets)
    {
        //var MainWindow = (MainWindow)Application.Current.MainWindow;
        foreach (var entry in MapViewer.MapModes)
        {
            entry.Value.Visibility = entry.Key == currentMapMode
                ? Visibility.Visible
                : Visibility.Hidden;
        }

        switch (currentMapMode)
        {
            case MapViewer.MapMode.Political: //Political
                mapmodeButton_Political.Source = assets.PoliticalOn;
                mapmodeButton_Provinces.Source = assets.ProvinceOff;
                mapmodeButton_Terrain.Source = assets.TerrainOff;
                break;
            case MapViewer.MapMode.Provincial: //Province Map
                mapmodeButton_Political.Source = assets.PoliticalOff;
                mapmodeButton_Provinces.Source = assets.ProvinceOn;
                mapmodeButton_Terrain.Source = assets.TerrainOff;
                break;
            case MapViewer.MapMode.Terrain: //Terrain
                mapmodeButton_Political.Source = assets.PoliticalOff;
                mapmodeButton_Provinces.Source = assets.ProvinceOff;
                mapmodeButton_Terrain.Source = assets.TerrainOn;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(currentMapMode), currentMapMode, null);
        }

        //D_
    }
}