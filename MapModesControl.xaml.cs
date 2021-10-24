using Paradox_Editor.A_Map_Functions;
using Paradox_Editor.C_Window_Functions;
using System.Windows;
using System.Windows.Controls;


namespace Paradox_Editor
{

    public partial class MapModesControl : UserControl
    {
        public MapModesControl()
        {
            InitializeComponent();
        }

        public static int CurrentMapMode { get; set; } //Political(0), Provinces(1), Terrain(2)
        ///The above can be changed as modes are added
        
        //GOAL: DARKEN THE Icon OF THE MODE THAT IS CURRENTLY ACTIVE, UNDARKEN THE REST.
        //SIMULTANIOUSLY SET THE MAP "MODE" TO BE BASED ON THE MODE THAT IS CLICKED/ACTIVE

        private void mapmodeButton_Political_Click(object sender, RoutedEventArgs e)
        {
            CurrentMapMode = 0;
            UpdateMapModeVisibility();
            GameSoundHandler.SoundHandler.PlayClickSound();
        }

        private void mapmodeButton_Provinces_Click(object sender, RoutedEventArgs e)
        {
            CurrentMapMode = 1;
            UpdateMapModeVisibility();
            GameSoundHandler.SoundHandler.PlayClickSound();
        }

        private void mapmodeButton_Terrain_Click(object sender, RoutedEventArgs e)
        {
            CurrentMapMode = 2;
            UpdateMapModeVisibility();
            GameSoundHandler.SoundHandler.PlayClickSound();
        }

        public static void UpdateMapModeVisibility() //Updates & Disables the other existing map modes
        {
            var MainWindow = (MainWindow)Application.Current.MainWindow;
            var assets = MenuVisualHandler.VisualHandler.UpdateMapModeButtonVisuals();

            if (CurrentMapMode == 0) //If Political is on
            {
                MainWindow.mapModeButtons.mapmodeButton_Political.Source = assets.PoliticalOn;
                MainWindow.mapModeButtons.mapmodeButton_Provinces.Source = assets.ProvinceOff;
                MainWindow.mapModeButtons.mapmodeButton_Terrain.Source = assets.TerrainOff;

                MainWindow.mapProvinces.Visibility = Visibility.Hidden;
                MainWindow.mapPolitical.Visibility = Visibility.Visible;

            }
            else if (CurrentMapMode == 1) //If Provinces is on
            {
                MainWindow.mapModeButtons.mapmodeButton_Political.Source = assets.PoliticalOff;
                MainWindow.mapModeButtons.mapmodeButton_Provinces.Source = assets.ProvinceOn;
                MainWindow.mapModeButtons.mapmodeButton_Terrain.Source = assets.TerrainOff;

                MainWindow.mapProvinces.Visibility = Visibility.Visible;
                MainWindow.mapPolitical.Visibility = Visibility.Hidden;

            }
            else if (CurrentMapMode == 2) //If Terrain is on
            {
                MainWindow.mapModeButtons.mapmodeButton_Political.Source = assets.PoliticalOff;
                MainWindow.mapModeButtons.mapmodeButton_Provinces.Source = assets.ProvinceOff;
                MainWindow.mapModeButtons.mapmodeButton_Terrain.Source = assets.TerrainOn;

                MainWindow.mapProvinces.Visibility = Visibility.Hidden;
                MainWindow.mapPolitical.Visibility = Visibility.Hidden;
            }


            //\\


        }
    }
}