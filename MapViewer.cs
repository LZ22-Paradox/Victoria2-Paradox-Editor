using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Paradox_Editor
{
    public partial class MainWindow : Window
    {

        private void mapmodeButton_Political_Click(object sender, RoutedEventArgs e)
        {
            string bitmapPath = @"/Preloaded_GFX/VIC2/mapmode_PoliticalOn.png";
            BitmapImage bitmapImage = new BitmapImage(new Uri(bitmapPath, UriKind.Relative));
            mapmodeButton_Political.Source = bitmapImage;

        }

        private void mapmodeButton_Provinces_Click(object sender, RoutedEventArgs e)
        {
            string bitmapPath = @"/Preloaded_GFX/VIC2/mapmode_ProvincesOn.png";
            BitmapImage bitmapImage = new BitmapImage(new Uri(bitmapPath, UriKind.Relative));
            mapmodeButton_Provinces.Source = bitmapImage;
        }

        private void mapmodeButton_Terrain_Click(object sender, RoutedEventArgs e)
        {
            string bitmapPath = @"/Preloaded_GFX/VIC2/mapmode_TerrainOn.png";
            BitmapImage bitmapImage = new BitmapImage(new Uri(bitmapPath, UriKind.Relative));
            mapmodeButton_Terrain.Source = bitmapImage;
        }








        //Impliment code for map editing and etc here





    }
}
