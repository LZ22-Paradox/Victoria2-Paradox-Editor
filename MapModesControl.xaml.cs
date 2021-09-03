using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Paradox_Editor
{
    /// <summary>
    /// Interaction logic for MapModes.xaml
    /// </summary>
    /// 

    public partial class MapModesControl : System.Windows.Controls.UserControl
    {

        public MapModesControl() => InitializeComponent();

        public int CurrentMapMode { get; set; } //Political(0), Provinces(1), Terrain(2)
        string bitmapPath = @"/Preloaded_GFX/VIC2/"; //The Resource Path for the Icons

        //GOAL: DARKEN THE ICON OF THE MODE THAT IS CURRENTLY ACTIVE, UNDARKEN THE REST.
        //SIMULTANIOUSLY SET THE MAP "MODE" TO BE BASED ON THE MODE THAT IS CLICKED/ACTIVE

        private void mapmodeButton_Political_Click(object sender, RoutedEventArgs e)
        {
            BitmapImage bitmapImage = new BitmapImage(new Uri(bitmapPath + "mapmode_PoliticalOn.png", UriKind.Relative));
            mapmodeButton_Provinces.Source = bitmapImage;

            CurrentMapMode = 0;
            updateMapMode();
        }

        private void mapmodeButton_Provinces_Click(object sender, RoutedEventArgs e)
        {
            BitmapImage bitmapImage = new BitmapImage(new Uri(bitmapPath + "mapmode_ProvincesOn.png", UriKind.Relative));
            mapmodeButton_Provinces.Source = bitmapImage;

            CurrentMapMode = 1;
            updateMapMode();
        }

        private void mapmodeButton_Terrain_Click(object sender, RoutedEventArgs e)
        {
            BitmapImage bitmapImage = new BitmapImage(new Uri(bitmapPath + "mapmode_TerrainOn.png", UriKind.Relative));
            mapmodeButton_Terrain.Source = bitmapImage;


            CurrentMapMode = 2;
            updateMapMode();
        }

        public void updateMapMode()
        {
            BitmapImage provinceoff = new BitmapImage(new Uri(bitmapPath + "mapmode_ProvincesOff.png", UriKind.Relative));
            BitmapImage politicaloff = new BitmapImage(new Uri(bitmapPath + "mapmode_PoliticalOff.png", UriKind.Relative));
            BitmapImage terrainoff = new BitmapImage(new Uri(bitmapPath + "mapmode_TerrainOff.png", UriKind.Relative));

            //  update/disable the other existing map modes
            if (CurrentMapMode == 0) //If Political is on
            {
                mapmodeButton_Provinces.Source = provinceoff;
                mapmodeButton_Terrain.Source = terrainoff;
            }
            else if (CurrentMapMode == 1) //If Provinces is on
            {
                mapmodeButton_Political.Source = politicaloff;
                mapmodeButton_Terrain.Source = terrainoff;
            }
            else if (CurrentMapMode == 2) //If Terrain is on
            {
                mapmodeButton_Political.Source = politicaloff;
                mapmodeButton_Provinces.Source = provinceoff;
            }
        }
    }
}