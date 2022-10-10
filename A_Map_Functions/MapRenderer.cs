using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Threading.Tasks;
using System;
using Paradox_Editor.A_Map_Functions;
using Paradox_Editor.C_Window_Functions;
using Paradox_Editor.D_Types;

namespace Paradox_Editor.A_Map_Navigation
{
    public class MapRenderer
    {
        private DataAcquisition ModData;

        public MapRenderer(DataAcquisition modData)
        {
            ModData = modData;
        }
        public MapRenderer() { }

        public uint GetRawColor(Color color) => (0xFFu << 24)
            | ((uint)color.R << 16) | ((uint)color.G << 8) | ((uint)color.B);

        /// <summary>
        /// Draws the political map.
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
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
                else
                {
                    if (ModData.GetProvinces().TryGetValue((uint)provinceID, out var provinceFile) == true)
                        pixels[index] = GetRawColor(Colors.Black); //Uncolonized
                    else
                        pixels[index] = GetRawColor(Colors.White); //Ocean
                }
                    
            });
            image.Unlock();
            return image;
        }

        public void DrawStateMap()
        {
            //States, aka "regions" are found in ...mod/map/region.txt whilst continent provinces are located in continent.txt
        }

        public void DrawCultureMap()
        {

        }

        public void DrawPopulationMap()
        {

        }

        public unsafe WriteableBitmap DrawSelectedProvince(WriteableBitmap image, uint color)
        {
            image.Lock();
            var pixels = (uint*)image.BackBuffer;
            var pixelCount = image.PixelWidth * image.PixelHeight;

            Parallel.For(0, pixelCount, (index) =>
            {
                var rawPixel = pixels[index];

                if (rawPixel == color)
                    pixels[index] = GetRawColor(Colors.WhiteSmoke);
                else
                    pixels[index] = 0;
            });

            image.Unlock();
            return image;
        }

        ///Begin work trying to get single province to update. Input specific province color, then look for it,
        ///then replace with province's TAG's color
        /*public unsafe WriteableBitmap ReRenderPoliticalProvince(WriteableBitmap image, ProvinceFile province)
        {
            image.Lock();
            var pixels = (uint*)image.BackBuffer;
            var pixelCount = image.PixelWidth * image.PixelHeight;

            ModData.GetColorsToProvinceIDs().TryGetValue(provinceColor, out var provinceID);
            ModData.GetProvinces().TryGetValue((uint)provinceID, out var province);
            ModData.GetTagsToCountryNames().TryGetValue(province.Owner, out var countryTAG);
            ModData.GetCountryNamesToColours().TryGetValue(countryTAG, out var oldCountryColor);
            var oldColor = GetRawColor(oldCountryColor);

            Parallel.For(0, pixelCount, (index) =>
            {
                var rawPixel = pixels[index];

                if (rawPixel == oldColor)
                {
                    pixels[index] = GetRawColor(newCountryColor);
                }

            });

            image.Unlock();
            return image;
    }*/

    }
}
