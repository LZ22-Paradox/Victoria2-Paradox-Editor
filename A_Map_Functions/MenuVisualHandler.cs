using Paradox_Editor.D_Types;
using Paradox_Editor.D_Types.Asset_Types;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Paradox_Editor.A_Map_Functions
{

    public class MenuVisualHandler
    {
        public static MenuVisualHandler VisualHandler { get; set; } = new MenuVisualHandler();

        public MainWindow MainWindow { get; set; } = (MainWindow)System.Windows.Application.Current.MainWindow;
        public string IconAssetsPath = @"/Preloaded_Assets/VIC2/Icons/"; //The Resource Path for the Icons
        public string ImageAssetsPath = @"/Preloaded_Assets/VIC2/Icons/"; //The Resource Path for the Icons



        public HandledAssets ConductAssetChange(string Gamemode)
        {
            var TextColor = Brushes.White;
            var BoxColor = Brushes.Black;

            if (Gamemode == "VIC2")
            {
                IconAssetsPath = @"/Preloaded_Assets/VIC2/Icons/";
                ImageAssetsPath = @"/Preloaded_Assets/VIC2/Images/";
                TextColor = Brushes.Black;
                BoxColor = Brushes.White;
            }
            else if (Gamemode == "EU4")
            {
                IconAssetsPath = @"/Preloaded_Assets/EU4/Icons/";
                ImageAssetsPath = @"/Preloaded_Assets/EU4/Images/";
                TextColor = Brushes.White;
                //#2B353C - Original EU4 Background
                BoxColor = (SolidColorBrush)new BrushConverter().ConvertFromString("#191F23");
            }
            else
            {
                IconAssetsPath = @"/Preloaded_Assets/VIC2/Icons/";
                ImageAssetsPath = @"/Preloaded_Assets/VIC2/Images/";
                TextColor = Brushes.Black;
                BoxColor = Brushes.White;
            }

            var politicaloff = new BitmapImage(new Uri(IconAssetsPath + "mapmode_PoliticalOff.png", UriKind.Relative));
            var politicalon = new BitmapImage(new Uri(IconAssetsPath + "mapmode_PoliticalOn.png", UriKind.Relative));
            var provinceoff = new BitmapImage(new Uri(IconAssetsPath + "mapmode_ProvincesOff.png", UriKind.Relative));
            var provinceon = new BitmapImage(new Uri(IconAssetsPath + "mapmode_ProvincesOn.png", UriKind.Relative));
            var terrainoff = new BitmapImage(new Uri(IconAssetsPath + "mapmode_TerrainOff.png", UriKind.Relative));
            var terrainon = new BitmapImage(new Uri(IconAssetsPath + "mapmode_TerrainOn.png", UriKind.Relative));

            var addicon = new BitmapImage(new Uri(IconAssetsPath + "button_add.png", UriKind.Relative));
            var removeicon = new BitmapImage(new Uri(IconAssetsPath + "button_remove.png", UriKind.Relative));
            var reseticon = new BitmapImage(new Uri(IconAssetsPath + "button_reset.png", UriKind.Relative));
            var exiticon = new BitmapImage(new Uri(IconAssetsPath + "button_exit.png", UriKind.Relative));

            var interfacebackground = new BitmapImage(new Uri(ImageAssetsPath + "provinceInterface.png", UriKind.Relative));

            //<---------------------------------------------->
            var MapModeIcons = new MapMode_IconSet
            {
                PoliticalOff = politicaloff,
                PoliticalOn = politicalon,
                ProvinceOff = provinceoff,
                ProvinceOn = provinceon,
                TerrainOff = terrainoff,
                TerrainOn = terrainon
            };
            var InterfaceAssets = new Interface_AssetSet
            {
                Add_Icon = addicon,
                Remove_Icon = removeicon,
                Reset_Icon = reseticon,
                Exit_Icon = exiticon,
                Interface_Background = interfacebackground,
                TextColor = TextColor,
                BoxColor = BoxColor
            };
            //<---------------------------------------------->

            return new HandledAssets()
            {
                MapModeIconSet = MapModeIcons,
                InterfaceAssetSet = InterfaceAssets
            };
        }

        public void UpdateInterface(HandledAssets InterfaceAssets)
        {
            MainWindow.FileInterface.background.Source = InterfaceAssets.InterfaceAssetSet.Interface_Background;
            MainWindow.FileInterface.AddCore_Image.Source = InterfaceAssets.InterfaceAssetSet.Add_Icon;
            MainWindow.FileInterface.ResetCore_Image.Source = InterfaceAssets.InterfaceAssetSet.Reset_Icon;
            MainWindow.FileInterface.ExitButton_Image.Source = InterfaceAssets.InterfaceAssetSet.Exit_Icon;

            MainWindow.FileInterface.PROVIDBOX.Foreground = InterfaceAssets.InterfaceAssetSet.TextColor;
            MainWindow.FileInterface.NAMEBOX.Foreground = InterfaceAssets.InterfaceAssetSet.TextColor;
            MainWindow.FileInterface.Color.Foreground = InterfaceAssets.InterfaceAssetSet.TextColor;
            MainWindow.FileInterface.COLORRGB.Foreground = InterfaceAssets.InterfaceAssetSet.TextColor;
            MainWindow.FileInterface.Owner.Foreground = InterfaceAssets.InterfaceAssetSet.TextColor;
            MainWindow.FileInterface.OWNERBOX.Foreground = InterfaceAssets.InterfaceAssetSet.TextColor;
            MainWindow.FileInterface.Controller.Foreground = InterfaceAssets.InterfaceAssetSet.TextColor;
            MainWindow.FileInterface.CONTROLLERBOX.Foreground = InterfaceAssets.InterfaceAssetSet.TextColor;
            MainWindow.FileInterface.TradeGood.Foreground = InterfaceAssets.InterfaceAssetSet.TextColor;
            MainWindow.FileInterface.TRADEGOODBOX.Foreground = InterfaceAssets.InterfaceAssetSet.TextColor;
            MainWindow.FileInterface.LifeRating.Foreground = InterfaceAssets.InterfaceAssetSet.TextColor;
            MainWindow.FileInterface.LIFERATINGBOX.Foreground = InterfaceAssets.InterfaceAssetSet.TextColor;
            MainWindow.FileInterface.Colonial.Foreground = InterfaceAssets.InterfaceAssetSet.TextColor;
            MainWindow.FileInterface.COLONIALBOX.Foreground = InterfaceAssets.InterfaceAssetSet.TextColor;
            MainWindow.FileInterface.Cores.Foreground = InterfaceAssets.InterfaceAssetSet.TextColor;

            SolidColorBrush backgroundColor = (SolidColorBrush)InterfaceAssets.InterfaceAssetSet.BoxColor;
            SolidColorBrush foregroundColor = (SolidColorBrush)InterfaceAssets.InterfaceAssetSet.TextColor;

            System.Windows.Application.Current.Resources["theBackgroundBrush"] = new SolidColorBrush(backgroundColor.Color);
            System.Windows.Application.Current.Resources["theForegroundBrush"] = new SolidColorBrush(foregroundColor.Color);

        }

    }

}
