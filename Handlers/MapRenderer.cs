using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Threading.Tasks;
using System;
using Paradox_Editor.D_Types;
using Paradox_Editor.Data_Handling;

namespace Paradox_Editor.Handlers
{
    public class MapRenderer
    {
        private ProvinceDataAcquisition ProvinceData;
        //private ProvinceDataAcquisition ProvinceData;
        //private CountryDataAcquisition CountryData;

        public MapRenderer(ProvinceDataAcquisition modData)
        {
            ProvinceData = modData;
        }
        public MapRenderer() { }

        public uint GetRawColor(Color color) => 0xFFu << 24
            | (uint)color.R << 16 | (uint)color.G << 8 | color.B;

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

                ProvinceData.GetColorsToProvinceIDs().TryGetValue(rawPixel, out var provinceID);
                var province = ProvinceData.GetProvince((uint)provinceID);

                bool foundOwner = false;

                if (!string.IsNullOrEmpty(province?.Owner))
                {
                    foundOwner = ModData.COUNTRY_DATA.GetCountries().TryGetValue(province.Owner, out var country);
                    pixels[index] = GetRawColor(country.GetColor());
                }
                else
                {
                    if (ProvinceData.GetProvinces().TryGetValue((uint)provinceID, out var provinceFile) == true) //Checking if province is History
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

            ProvinceData.GetColorsToProvinceIDs().TryGetValue(provinceColor, out var provinceID);
            var province = ProvinceData.GetProvince((uint)provinceID);

            Color countryColor;

            if (province.Owner != null)
            {
                ModData.COUNTRY_DATA.GetCountries().TryGetValue(province.Owner, out var country);
                countryColor = country.GetColor();
            }
            else
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
