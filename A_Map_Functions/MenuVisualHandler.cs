using Paradox_Editor.D__Static_Classes_Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Paradox_Editor.A_Map_Functions
{

    public class MenuVisualHandler
    {
        public static MenuVisualHandler VisualHandler { get; set; } = new MenuVisualHandler();

        public string IconAssetsPath = @"/Preloaded_Assets/VIC2/Icons/"; //The Resource Path for the Icons
        public string ImageAssetsPath = @"/Preloaded_Assets/VIC2/Icons/"; //The Resource Path for the Icons


        public MapMode_IconSet ConductAssetChange(string Gamemode)
        {
            if (Gamemode == "VIC2")
            {
                IconAssetsPath = @"/Preloaded_Assets/VIC2/Icons/";
                ImageAssetsPath = @"/Preloaded_Assets/VIC2/Images/";
            }
            else if (Gamemode == "EU4")
            {
                IconAssetsPath = @"/Preloaded_Assets/EU4/Icons/";
                ImageAssetsPath = @"/Preloaded_Assets/EU4/Images/";
            }
            else
            {
                IconAssetsPath = @"/Preloaded_Assets/VIC2/Icons/";
                ImageAssetsPath = @"/Preloaded_Assets/VIC2/Images/";
            }

            var MainWindow = (MainWindow)Application.Current.MainWindow;

            BitmapImage politicaloff = new BitmapImage(new Uri(IconAssetsPath + "mapmode_PoliticalOff.png", UriKind.Relative));
            BitmapImage politicalon = new BitmapImage(new Uri(IconAssetsPath + "mapmode_PoliticalOn.png", UriKind.Relative));
            BitmapImage provinceoff = new BitmapImage(new Uri(IconAssetsPath + "mapmode_ProvincesOff.png", UriKind.Relative));
            BitmapImage provinceon = new BitmapImage(new Uri(IconAssetsPath + "mapmode_ProvincesOn.png", UriKind.Relative));
            BitmapImage terrainoff = new BitmapImage(new Uri(IconAssetsPath + "mapmode_TerrainOff.png", UriKind.Relative));
            BitmapImage terrainon = new BitmapImage(new Uri(IconAssetsPath + "mapmode_TerrainOn.png", UriKind.Relative));

            MainWindow.FileInterface.background.Source = new BitmapImage(new Uri(ImageAssetsPath + "provinceInterface.png", UriKind.Relative));

            return new MapMode_IconSet()
            {
                PoliticalOff = politicaloff,
                PoliticalOn = politicalon,
                ProvinceOff = provinceoff,
                ProvinceOn = provinceon,
                TerrainOff = terrainoff,
                TerrainOn = terrainon
            };
        }
    }

}
