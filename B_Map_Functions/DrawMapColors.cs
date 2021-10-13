using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Paradox_Editor.A_Map_Navigation
{
    public class DrawMapColors
    {

        private Image Image;
        private WriteableBitmap FirstLayer;
        public WriteableBitmap SizeReference { get; set; }

        private Dictionary<string, string> Dictionary1;
        private Dictionary<string, string> Dictionary2;
        private Dictionary<string, string> Dictionary3;
        private Dictionary<string, Color> Dictionary4;


        public DrawMapColors(WriteableBitmap firstlayer, WriteableBitmap sizereference, Image image, Dictionary<string, string> dictionary1, Dictionary<string, string> dictionary2, Dictionary<string, string> dictionary3, Dictionary<string, Color> dictionary4)
        {
            Image = image;
            FirstLayer = firstlayer;
            SizeReference = sizereference;
            Dictionary1 = dictionary1; //colorToProvinceId
            Dictionary2 = dictionary2; //provinceToCountry
            Dictionary3 = dictionary3; //countryToColor
            Dictionary4 = dictionary4;
        }

        public void Drawing()
        {
            //var colorToProvindeID = Dictionary; || Find dictionary's use for trygetValue. Possible new class?

            for (int x = 0; x < FirstLayer.Width; x++) //REUSE LATER. STATIC CLASS BAD
            {
                for (int y = 0; y < FirstLayer.Height; y++)
                {
                    var pixel = FirstLayer.GetPixel(x, y);
/*                    if (
                        Dictionary1.TryGetValue(pixel.R + " " + pixel.G + " " + pixel.B, out var provinceID) &&
                        Dictionary2.TryGetValue(provinceID, out var country) &&
                        Dictionary3.TryGetValue(country, out var countryColor))
                    {
                        SizeReference.FillRectangle(x, y, x + 1, y + 1, countryColor); //Draws the country colors
                    }
                    else
                    {
                        SizeReference.FillRectangle(x, y, x + 1, y + 1, pixel); //Draws the province colors
                    }*/

///This is the area of significant change for next update. See the IF statement parameters with the new dictionary entries.

                }

            }

        }

    }
}
