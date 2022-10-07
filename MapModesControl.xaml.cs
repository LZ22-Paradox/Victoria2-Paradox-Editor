using Paradox_Editor.C_Window_Functions;
using Paradox_Editor.D_Types;
using System.Collections.Generic;
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

        private int CurrentMapMode = 1;//Political(0), Provinces(1), Terrain(2)

        public int GetMapMode() => CurrentMapMode;

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

        private void MapmodeButton_Terrain_Click(object sender, RoutedEventArgs e)
        {
            CurrentMapMode = 2;
            UpdateMapModeVisibility(CurrentMapMode, VisualHandler.MapModeIconSet);
            SoundHandler.PlayClickSound();
        }

        public void UpdateMapModeVisibility(int CurrentMapMode, MapMode_IconSet Assets) //Updates & Disables the other existing map modes
        {
            //var MainWindow = (MainWindow)Application.Current.MainWindow;
            foreach (var entry in MapViewer.MapModes)
            {
                if (entry.Key == CurrentMapMode)
                    entry.Value.Visibility = Visibility.Visible;
                else
                    entry.Value.Visibility = Visibility.Hidden;
            }

            switch (CurrentMapMode)
            {
                case 0: //Political
                    mapmodeButton_Political.Source = Assets.PoliticalOn;
                    mapmodeButton_Provinces.Source = Assets.ProvinceOff;
                    mapmodeButton_Terrain.Source = Assets.TerrainOff;
                    break;
                case 1: //Province Map
                    mapmodeButton_Political.Source = Assets.PoliticalOff;
                    mapmodeButton_Provinces.Source = Assets.ProvinceOn;
                    mapmodeButton_Terrain.Source = Assets.TerrainOff;
                    break;
                case 2: //Terrain
                    mapmodeButton_Political.Source = Assets.PoliticalOff;
                    mapmodeButton_Provinces.Source = Assets.ProvinceOff;
                    mapmodeButton_Terrain.Source = Assets.TerrainOn;
                    break;
            }

            //D_
            
        }
    }
}