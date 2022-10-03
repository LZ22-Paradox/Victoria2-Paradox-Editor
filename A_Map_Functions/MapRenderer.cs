using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Threading.Tasks;
using System;
using Paradox_Editor.A_All_New_Methods;

namespace Paradox_Editor.A_Map_Navigation
{
    public class MapRenderer
    {

        private Image Image;
        public WriteableBitmap FirstLayer;
        public WriteableBitmap SizeReference { get; set; }

        private Dictionary<uint, string> Dictionary1;
        private Dictionary<string, string> Dictionary2;
        private Dictionary<string, string> Dictionary3;
        private Dictionary<string, Color> Dictionary4;
        private DataAcquisition ModData;

        public MapRenderer(DataAcquisition modData) => ModData = modData;

        [Obsolete]
        public MapRenderer(WriteableBitmap firstlayer, WriteableBitmap sizereference, Image image, Dictionary<uint, string> colorToProvinceID, Dictionary<string, string> provinceIDToOwnerTAG, Dictionary<string, string> countryTAGToCountryName, Dictionary<string, Color> countryToColor)
        {
            Image = image;
            FirstLayer = firstlayer;
            SizeReference = sizereference;
            Dictionary1 = colorToProvinceID;
            Dictionary2 = provinceIDToOwnerTAG;
            Dictionary3 = countryTAGToCountryName;
            Dictionary4 = countryToColor;

            //colorToProvinceId, provinceIDToControllerTAG, tagToCountryName, countryNameToColor
        }

        static uint GetRawColor(Color color) =>(0xFFu << 24)
            | ((uint)color.R << 16) | ((uint)color.G << 8) | ((uint)color.B);

        public unsafe void DrawProvinceMap()
        {
            FirstLayer.Lock();
            var pixels = (uint*)FirstLayer.BackBuffer;

            var pixelCount = FirstLayer.PixelWidth * FirstLayer.PixelHeight;

            Parallel.For(0, pixelCount, (index) =>
            {
                var rawPixel = pixels[index];

                if (
                    Dictionary1.TryGetValue(rawPixel, out var provinceID) &&
                    Dictionary2.TryGetValue(provinceID, out var countryTAG) &&
                    Dictionary3.TryGetValue(countryTAG, out var countryName) &&
                    Dictionary4.TryGetValue(countryName, out var countryColor))
                {
                    pixels[index] = GetRawColor(countryColor);
                }
                else if (Dictionary1.TryGetValue(rawPixel, out var UncolonizedID)
                    && !Dictionary2.TryGetValue(provinceID, out var unColonizedTag))
                {
                    pixels[index] = GetRawColor(Colors.White);
                }
                else
                {
                    pixels[index] = GetRawColor(Colors.Black); //Uncolonized
                }

                ///This is the area for significant change; different modes.
            });

            FirstLayer.Unlock();
            SizeReference = FirstLayer;
        }

        public void DrawStateMap()
        {

        }

        public void DrawCultureMap()
        {

        }

        public void DrawPopulationMap()
        {

        }

    }
}
