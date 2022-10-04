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

        public WriteableBitmap SizeReference { get; set; }

        private DataAcquisition ModData;

        /*        [Obsolete]
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
                }*/

        public MapRenderer(DataAcquisition modData)
        {
            ModData = modData;
        }

        static uint GetRawColor(Color color) =>(0xFFu << 24)
            | ((uint)color.R << 16) | ((uint)color.G << 8) | ((uint)color.B);

        public unsafe WriteableBitmap DrawPoliticalMap(WriteableBitmap image)
        {
            image.Lock();
            var pixels = (uint*)image.BackBuffer;

            var pixelCount = image.PixelWidth * image.PixelHeight;

            Parallel.For(0, pixelCount, (index) =>
            {
                var rawPixel = pixels[index];

                ModData.GetColorsToProvinceIDs().TryGetValue(rawPixel, out var provinceID);
                ModData.GetProvinces().TryGetValue((uint)provinceID, out var province);

                bool foundOwner = false;

                if (province?.Owner != null)
                {
                    foundOwner = ModData.GetTagsToCountryNames().TryGetValue(province.Owner, out var countryTAG);
                    ModData.GetCountryNamesToColours().TryGetValue(countryTAG, out var countryColor);
                    pixels[index] = GetRawColor(countryColor);
                } 
                else if (province?.Owner == null)
                if (ModData.GetProvinces().TryGetValue((uint)provinceID, out var provinceFile) == true)
                        pixels[index] = GetRawColor(Colors.Black); //Uncolonized
                else
                        pixels[index] = GetRawColor(Colors.White); //Ocean
            });

            image.Unlock();
            return image;
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
