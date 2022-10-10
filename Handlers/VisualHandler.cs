using Paradox_Editor.D_Types;
using Paradox_Editor.D_Types.Assets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Paradox_Editor.C_Window_Functions
{
    public static class VisualHandler
    {
        public static MapMode_IconSet MapModeIconSet { get; set; }
        public static Interface_AssetSet InterfaceAssetSet { get; set; }

        private static string IconAssetsPath = @"/Assets/VIC2/Icons/"; //The Resource Path for the Icons
        private static string ImageAssetsPath = @"/Assets/VIC2/Icons/"; //The Resource Path for the Icons

        //public MainWindow MainWindow { get; set; } = (MainWindow)System.Windows.Application.Current.MainWindow;


        public static void ConductAssetChange(string Gamemode)
        {
            Brush textColor = Brushes.Black;
            Brush boxColor = Brushes.White;

            switch (Gamemode)
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
                    boxColor = (SolidColorBrush)new BrushConverter().ConvertFromString("#191F23"); //#2B353C - EU4 Background
                    break;
                default:
                    IconAssetsPath = @"/Assets/VIC2/Icons/";
                    ImageAssetsPath = @"/Assets/VIC2/Images/";
                    textColor = Brushes.Black;
                    boxColor = Brushes.White;
                    break;
            }

            //<---------------------------------------------->

            MapModeIconSet = new()
            {
                PoliticalOff = new BitmapImage(new Uri(IconAssetsPath + "mapmode_PoliticalOff.png", UriKind.Relative)),
                PoliticalOn = new BitmapImage(new Uri(IconAssetsPath + "mapmode_PoliticalOn.png", UriKind.Relative)),
                ProvinceOff = new BitmapImage(new Uri(IconAssetsPath + "mapmode_ProvincesOff.png", UriKind.Relative)),
                ProvinceOn = new BitmapImage(new Uri(IconAssetsPath + "mapmode_ProvincesOn.png", UriKind.Relative)),
                TerrainOff = new BitmapImage(new Uri(IconAssetsPath + "mapmode_TerrainOff.png", UriKind.Relative)),
                TerrainOn = new BitmapImage(new Uri(IconAssetsPath + "mapmode_TerrainOn.png", UriKind.Relative))
            };

            InterfaceAssetSet = new()
            {
                Add_Icon = new BitmapImage(new Uri(IconAssetsPath + "button_add.png", UriKind.Relative)),
                Remove_Icon = new BitmapImage(new Uri(IconAssetsPath + "button_remove.png", UriKind.Relative)),
                Reset_Icon = new BitmapImage(new Uri(IconAssetsPath + "button_reset.png", UriKind.Relative)),
                Exit_Icon = new BitmapImage(new Uri(IconAssetsPath + "button_exit.png", UriKind.Relative)),

                Interface_Background = new BitmapImage(new Uri(ImageAssetsPath + "provinceInterface.png", UriKind.Relative)),

                TextColor = textColor,
                BoxColor = boxColor
            };

            SolidColorBrush backgroundColor = (SolidColorBrush)InterfaceAssetSet.BoxColor;
            SolidColorBrush foregroundColor = (SolidColorBrush)InterfaceAssetSet.TextColor;
            Application.Current.Resources["theBackgroundBrush"] = new SolidColorBrush(backgroundColor.Color);
            Application.Current.Resources["theForegroundBrush"] = new SolidColorBrush(foregroundColor.Color);

        }

    }

}
