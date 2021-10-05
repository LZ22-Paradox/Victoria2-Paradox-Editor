using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Paradox_Editor.A_Map_Navigation
{
    public class DrawMapColors
    {

        private Image Image;
        private Dictionary<string, string> Dictionary;

        public DrawMapColors(Image image, Dictionary<string, string> dictionary)
        {
            Image = image;
            Dictionary = dictionary;
        }

        public void Drawing(object sender)
        {

            var firstLayer = BitmapFactory.ConvertToPbgra32Format((BitmapSource)Image.Source);
            var writeableBmp = BitmapFactory.New((int)firstLayer.Width, (int)firstLayer.Height);
            writeableBmp.Clear(Colors.White);

            //var colorToProvindeID = Dictionary; || Find dictionary's use for trygetValue. Possible new class?

/*            for (int x = 0; x < firstLayer.Width; x++) //REUSE LATER. STATIC CLASS BAD
            {
                for (int y = 0; y < firstLayer.Height; y++)
                {
                    var pixel = firstLayer.GetPixel(x, y);
                    if (
                        colorToProvinceId.TryGetValue(pixel.R + " " + pixel.G + " " + pixel.B, out var provinceID) &&
                        provinceToCountry.TryGetValue(provinceID, out var country) &&
                        countryToColor.TryGetValue(country, out var countryColor))
                    {
                        writeableBmp.FillRectangle(x, y, x + 1, y + 1, countryColor); //Draws the country colors
                    }
                    else
                    {
                        writeableBmp.FillRectangle(x, y, x + 1, y + 1, pixel); //Draws the province colors
                    }

                }

            }*/

        }

    }
}
