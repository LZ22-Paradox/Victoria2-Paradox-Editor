using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Paradox_Editor.Extensions.Assets;

namespace Paradox_Editor.Handlers;

public static class VisualHandler
{
    public static MapMode_IconSet MapModeIconSet { get; set; } = null!;
    public static Interface_AssetSet InterfaceAssetSet { get; set; } = null!;

    private static string IconAssetsPath = @"/Assets/VIC2/Icons/"; //The Resource Path for the Icons
    private static string ImageAssetsPath = @"/Assets/VIC2/Icons/"; //The Resource Path for the Icons

    //public MainWindow MainWindow { get; set; } = (MainWindow)System.Windows.Application.Current.MainWindow;

    public static void ConductAssetChange(string gameMode)
    {
        Brush textColor;
        Brush boxColor;

        switch (gameMode)
        {
            case "VIC2":
                IconAssetsPath = @"/Assets/VIC2/Icons/";
                ImageAssetsPath = @"/Assets/VIC2/Images/";
                textColor = Brushes.Black;
                boxColor = Brushes.White;
                break;
            case "EU4":
                IconAssetsPath = @"/Assets/EU4/Icons/";
                ImageAssetsPath = @"/Assets/EU4/Images/";
                textColor = Brushes.White;
                boxColor = (SolidColorBrush)new BrushConverter().ConvertFromString("#191F23")!; //#2B353C - EU4 Background
                break;
            default:
                IconAssetsPath = @"/Assets/VIC2/Icons/";
                ImageAssetsPath = @"/Assets/VIC2/Images/";
                textColor = Brushes.Black;
                boxColor = Brushes.White;
                break;
        }

        //<---------------------------------------------->

        MapModeIconSet = new MapMode_IconSet
        {
            PoliticalOff = new BitmapImage(new Uri($"{IconAssetsPath}mapmode_PoliticalOff.png", UriKind.Relative)),
            PoliticalOn = new BitmapImage(new Uri($"{IconAssetsPath}mapmode_PoliticalOn.png", UriKind.Relative)),
            ProvinceOff = new BitmapImage(new Uri($"{IconAssetsPath}mapmode_ProvincesOff.png", UriKind.Relative)),
            ProvinceOn = new BitmapImage(new Uri($"{IconAssetsPath}mapmode_ProvincesOn.png", UriKind.Relative)),
            TerrainOff = new BitmapImage(new Uri($"{IconAssetsPath}mapmode_TerrainOff.png", UriKind.Relative)),
            TerrainOn = new BitmapImage(new Uri($"{IconAssetsPath}mapmode_TerrainOn.png", UriKind.Relative))
        };

        InterfaceAssetSet = new Interface_AssetSet
        {
            AddIcon = new BitmapImage(new Uri($"{IconAssetsPath}button_add.png", UriKind.Relative)),
            RemoveIcon = new BitmapImage(new Uri($"{IconAssetsPath}button_remove.png", UriKind.Relative)),
            ResetIcon = new BitmapImage(new Uri($"{IconAssetsPath}button_reset.png", UriKind.Relative)),
            ExitIcon = new BitmapImage(new Uri($"{IconAssetsPath}button_exit.png", UriKind.Relative)),
            InterfaceBackground = new BitmapImage(new Uri($"{ImageAssetsPath}provinceInterface.png", UriKind.Relative)),
            TextColor = textColor,
            BoxColor = boxColor
        };

        var backgroundColor = (SolidColorBrush)InterfaceAssetSet.BoxColor;
        var foregroundColor = (SolidColorBrush)InterfaceAssetSet.TextColor;
        Application.Current.Resources["theBackgroundBrush"] = new SolidColorBrush(backgroundColor.Color);
        Application.Current.Resources["theForegroundBrush"] = new SolidColorBrush(foregroundColor.Color);

    }

}