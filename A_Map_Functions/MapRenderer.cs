using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Paradox_Editor.A_Map_Navigation
{
    public class MapRenderer
    {

        private Image Image;
        public WriteableBitmap FirstLayer;
        public WriteableBitmap SizeReference { get; set; }

        private Dictionary<string, string> Dictionary1;
        private Dictionary<string, string> Dictionary2;
        private Dictionary<string, string> Dictionary3;
        private Dictionary<string, Color> Dictionary4;


        public MapRenderer(WriteableBitmap firstlayer, WriteableBitmap sizereference, Image image, Dictionary<string, string> dictionary1, Dictionary<string, string> dictionary2, Dictionary<string, string> dictionary3, Dictionary<string, Color> dictionary4)
        {
            Image = image;
            FirstLayer = firstlayer;
            SizeReference = sizereference;
            Dictionary1 = dictionary1; //colorToProvinceId
            Dictionary2 = dictionary2; //provinceIDToControllerTAG
            Dictionary3 = dictionary3; //countryTAGToCountryName
            Dictionary4 = dictionary4; //countryToColor

            //colorToProvinceId, provinceIDToControllerTAG, tagToCountryName, countryNameToColor
        }

        public void Drawing()
        {
            for (int x = 0; x < FirstLayer.PixelWidth - 1; x++)
            {
                for (int y = 0; y < FirstLayer.PixelHeight - 1; y++)
                {
                    var pixel = FirstLayer.GetPixel(x, y); //Error is thrown here
                    if (
                        Dictionary1.TryGetValue(pixel.R + " " + pixel.G + " " + pixel.B, out var provinceID) &&
                        Dictionary2.TryGetValue(provinceID, out var countryTAG) &&
                        Dictionary3.TryGetValue(countryTAG, out var countryName) &&
                        Dictionary4.TryGetValue(countryName, out var countryColor))
                    {
                        SizeReference.FillRectangle(x, y, x + 1, y + 1, countryColor); //Draws the country colors
                    }
                    else if (Dictionary1.TryGetValue(pixel.R + " " + pixel.G + " " + pixel.B, out var UncolonizedID) && !Dictionary2.TryGetValue(provinceID, out var unColonizedTag))
                    {
                        SizeReference.FillRectangle(x, y, x + 1, y + 1, Color.FromRgb(255, 255, 255)); //Draw missing data in black
                    } else
                    {
                        SizeReference.FillRectangle(x, y, x + 1, y + 1, Color.FromRgb(0, 0, 0)); //Draw missing data in black


                    }

                    ///This is the area of significant change for next update. See the IF statement parameters with the new dictionary entries.

                }

            }

        }

    }
}
