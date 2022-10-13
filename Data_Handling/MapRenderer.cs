using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Threading.Tasks;
using System;
using Paradox_Editor.D_Types;

namespace Paradox_Editor.Data_Handling
{
    public class MapRenderer
    {
        private ProvinceDataAcquisition ModData;

        public MapRenderer(ProvinceDataAcquisition modData)
        {
            ModData = modData;
        }
        public MapRenderer() { }

        public uint GetRawColor(Color color) => (0xFFu << 24)
            | ((uint)color.R << 16) | ((uint)color.G << 8) | ((uint)color.B);

        #region Drawing Navigatable Maps
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
                var province = ModData.GetProvince((uint)provinceID);

                bool foundOwner = false;
                
                if (!String.IsNullOrEmpty(province?.Owner))
                {
                    foundOwner = ModData.GetTagsToCountryNames().TryGetValue(province.Owner, out var countryTAG);
                    ModData.GetCountryNamesToColours().TryGetValue(countryTAG, out var countryColor);
                    pixels[index] = GetRawColor(countryColor);
                }
                else
                {
                    if (ModData.GetProvinces().TryGetValue((uint)provinceID, out var provinceFile) == true) //Checking if province is History
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
        #endregion

        /// <summary>
        /// Redraws the given province that has been selected.
        /// </summary>
        /// <param name="image"></param>
        /// <param name="color"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Refreshes a single province in the political map. Could be rewritten in the future to accomodate for other map types.
        /// </summary>
        /// <param name="provinceMap"></param>
        /// <param name="overWrittenMap"></param>
        /// <param name="provinceColor"></param>
        /// <returns></returns>
        public unsafe WriteableBitmap RefreshProvincePolitical(WriteableBitmap provinceMap, WriteableBitmap overWrittenMap, uint provinceColor)
        {
            provinceMap.Lock();
            overWrittenMap.Lock();
            var givenPixels = (uint*)provinceMap.BackBuffer;
            var overWrittenPixels = (uint*)overWrittenMap.BackBuffer;
            var pixelCount = provinceMap.PixelWidth * provinceMap.PixelHeight;

            ModData.GetColorsToProvinceIDs().TryGetValue(provinceColor, out var provinceID);
            var province = ModData.GetProvince((uint)provinceID);

            Color countryColor;

            if (province.Owner != null)
            {
                ModData.GetTagsToCountryNames().TryGetValue(province.Owner, out var countryTAG);
                ModData.GetCountryNamesToColours().TryGetValue(countryTAG, out countryColor);
            } else
                countryColor = Colors.Black;

            var newColor = GetRawColor(countryColor);

            Parallel.For(0, pixelCount, (index) =>
            {
                var rawPixel = givenPixels[index];

                if (rawPixel == provinceColor)
                {
                    overWrittenPixels[index] = newColor;
                }
            });

            provinceMap.Unlock();
            overWrittenMap.Unlock();

            return overWrittenMap;
        }

    }
}
