using Paradox_Editor.C_Window_Functions;
using System;
using System.Media;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Paradox_Editor
{

    public partial class MapModesControl : System.Windows.Controls.UserControl
    {
        public static int CurrentMapMode { get; set; } //Political(0), Provinces(1), Terrain(2)
        public static string bitmapPath = @"/Preloaded_Assets/VIC2/Icons/"; //The Resource Path for the Icons
        ///The above can be changed as modes are added

        public MapModesControl() => InitializeComponent();


        //GOAL: DARKEN THE ICON OF THE MODE THAT IS CURRENTLY ACTIVE, UNDARKEN THE REST.
        //SIMULTANIOUSLY SET THE MAP "MODE" TO BE BASED ON THE MODE THAT IS CLICKED/ACTIVE

        private void mapmodeButton_Political_Click(object sender, RoutedEventArgs e)
        {
            CurrentMapMode = 0;
            updateMapModeVisibility();
            var SoundHandler = new GameSoundHandler();//Work on special sound handler later
            SoundHandler.PlayClickSound();
        }

        private void mapmodeButton_Provinces_Click(object sender, RoutedEventArgs e)
        {
            CurrentMapMode = 1;
            updateMapModeVisibility();
            var SoundHandler = new GameSoundHandler();//Work on special sound handler later
            SoundHandler.PlayClickSound();
        }

        private void mapmodeButton_Terrain_Click(object sender, RoutedEventArgs e)
        {
            CurrentMapMode = 2;
            updateMapModeVisibility();
            var SoundHandler = new GameSoundHandler();//Work on special sound handler later
            SoundHandler.PlayClickSound();
        }

        public static void updateMapModeVisibility() //Updates & Disables the other existing map modes
        {
            var MainWindow = (MainWindow)Application.Current.MainWindow;

            BitmapImage politicaloff = new BitmapImage(new Uri(bitmapPath + "mapmode_PoliticalOff.png", UriKind.Relative));
            BitmapImage politicalon = new BitmapImage(new Uri(bitmapPath + "mapmode_PoliticalOn.png", UriKind.Relative));

            BitmapImage provinceoff = new BitmapImage(new Uri(bitmapPath + "mapmode_ProvincesOff.png", UriKind.Relative));
            BitmapImage provinceon = new BitmapImage(new Uri(bitmapPath + "mapmode_ProvincesOn.png", UriKind.Relative));

            BitmapImage terrainoff = new BitmapImage(new Uri(bitmapPath + "mapmode_TerrainOff.png", UriKind.Relative));
            BitmapImage terrainon = new BitmapImage(new Uri(bitmapPath + "mapmode_TerrainOn.png", UriKind.Relative));

            //Simplify code to alleviate repeating. A foreach loop might work after binding each button
            //with an "on/off" property.
            if (CurrentMapMode == 0) //If Political is on
            {
                MainWindow.mapModeButtons.mapmodeButton_Political.Source = politicalon;
                MainWindow.mapModeButtons.mapmodeButton_Provinces.Source = provinceoff;
                MainWindow.mapModeButtons.mapmodeButton_Terrain.Source = terrainoff;

                MainWindow.mapBackground.Visibility = Visibility.Hidden;
                MainWindow.mapPolitical.Visibility = Visibility.Visible;

            }
            else if (CurrentMapMode == 1) //If Provinces is on
            {
                MainWindow.mapModeButtons.mapmodeButton_Political.Source = politicaloff;
                MainWindow.mapModeButtons.mapmodeButton_Provinces.Source = provinceon;
                MainWindow.mapModeButtons.mapmodeButton_Terrain.Source = terrainoff;

                MainWindow.mapBackground.Visibility = Visibility.Visible;
                MainWindow.mapPolitical.Visibility = Visibility.Hidden;

            }
            else if (CurrentMapMode == 2) //If Terrain is on
            {
                MainWindow.mapModeButtons.mapmodeButton_Political.Source = politicaloff;
                MainWindow.mapModeButtons.mapmodeButton_Provinces.Source = provinceoff;
                MainWindow.mapModeButtons.mapmodeButton_Terrain.Source = terrainon;

                MainWindow.mapBackground.Visibility = Visibility.Hidden;
                MainWindow.mapPolitical.Visibility = Visibility.Hidden;
            }


            //\\


        }
    }
}