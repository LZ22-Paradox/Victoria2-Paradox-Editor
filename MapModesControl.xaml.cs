using Paradox_Editor.C_Window_Functions;
using Paradox_Editor.D_Types;
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

        public int CurrentMapMode = 1;//Political(0), Provinces(1), Terrain(2)

        private void MapmodeButton_Political_Click(object sender, RoutedEventArgs e)
        {
            CurrentMapMode = 0;
            UpdateMapModeVisibility(CurrentMapMode, VisualHandler.MapModeIconSet);
            SoundHandler.PlayClickSound();
        }

        private void MapmodeButton_Provinces_Click(object sender, RoutedEventArgs e)
        {
            CurrentMapMode = 1;
            UpdateMapModeVisibility(CurrentMapMode, VisualHandler.MapModeIconSet);
            SoundHandler.PlayClickSound();
        }

        private void mapmodeButton_Terrain_Click(object sender, RoutedEventArgs e)
        {
            CurrentMapMode = 2;
            UpdateMapModeVisibility(CurrentMapMode, VisualHandler.MapModeIconSet);
            SoundHandler.PlayClickSound();
        }

        ///Do performance Update
        public static void UpdateMapModeVisibility(int CurrentMapMode, MapMode_IconSet Assets) //Updates & Disables the other existing map modes
        {
            var MainWindow = (MainWindow)Application.Current.MainWindow;
             
            if (CurrentMapMode == 0) //If Political is on
            {
                MainWindow.mapModeButtons.mapmodeButton_Provinces.Source = Assets.ProvinceOff;
                MainWindow.mapModeButtons.mapmodeButton_Political.Source = Assets.PoliticalOn;
                MainWindow.mapModeButtons.mapmodeButton_Terrain.Source = Assets.TerrainOff;

                MainWindow.mapPolitical.Visibility = Visibility.Visible;
                MainWindow.mapProvinces.Visibility = Visibility.Hidden;
                MainWindow.mapTerrain.Visibility = Visibility.Hidden;
            }
            else if (CurrentMapMode == 1) //If Provinces is on
            {
                MainWindow.mapModeButtons.mapmodeButton_Provinces.Source = Assets.ProvinceOn;
                MainWindow.mapModeButtons.mapmodeButton_Political.Source = Assets.PoliticalOff;
                MainWindow.mapModeButtons.mapmodeButton_Terrain.Source = Assets.TerrainOff;

                MainWindow.mapPolitical.Visibility = Visibility.Hidden;
                MainWindow.mapProvinces.Visibility = Visibility.Visible;
                MainWindow.mapTerrain.Visibility = Visibility.Hidden;
            }
            else if (CurrentMapMode == 2) //If Terrain is on
            {
                MainWindow.mapModeButtons.mapmodeButton_Provinces.Source = Assets.ProvinceOff;
                MainWindow.mapModeButtons.mapmodeButton_Political.Source = Assets.PoliticalOff;
                MainWindow.mapModeButtons.mapmodeButton_Terrain.Source = Assets.TerrainOn;

                MainWindow.mapPolitical.Visibility = Visibility.Hidden;
                MainWindow.mapProvinces.Visibility = Visibility.Hidden;
                MainWindow.mapTerrain.Visibility = Visibility.Visible;

            }


            //\\


        }
    }
}