using System;
using Paradox_Editor.Data_Handling;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Paradox_Editor.Extensions.Types;

namespace Paradox_Editor.Handlers;

public class MapRenderer
{
    private readonly ProvinceDataAcquisition ProvinceData;

    public MapRenderer(ProvinceDataAcquisition modData)
    {
        ProvinceData = modData;
    }

    public MapRenderer()
    {
    }

    public static uint GetRawColor(Color color) => 0xFFu << 24 | (uint)color.R << 16 | (uint)color.G << 8 | color.B;

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
            if (pixels == null)
                return; // No null pointer exceptions, today!

            var rawPixel = pixels[index];

            //Below being simplified
            ProvinceData.GetColorsToProvinceIDs().TryGetValue(rawPixel, out var provinceID);
            ProvinceFile province = ProvinceData.GetProvince((uint)provinceID);

            if (!string.IsNullOrEmpty(province?.Owner))
            {
                ModData.COUNTRY_DATA.GetCountries().TryGetValue(province.Owner, out Country country);
                // Some tags are problematic; see Heirs to Aquitania. May be related to the lack of a Fallback.
                if (country != null) pixels[index] = GetRawColor(country.GetColor());
            }
            else
            {
                // Checking if province is History
                if (ProvinceData.GetProvinces().TryGetValue((uint)provinceID, out ProvinceFile _))
                    pixels[index] = GetRawColor(Colors.Black); // Uncolonized
                else
                    pixels[index] = GetRawColor(Colors.White); // Ocean
            }
        });
        image.Unlock();
        return image;
    }

    public void DrawStateMap()
    {
        // States, aka "regions" are found in ...mod/map/region.txt whilst continent provinces are located in continent.txt
        throw new NotImplementedException();
    }

    public void DrawCultureMap()
    {
        throw new NotImplementedException();
    }

    public void DrawPopulationMap()
    {
        throw new NotImplementedException();
    }

    #endregion

    /// <summary>
    /// Redraws the given province that has been selected.
    /// </summary>
    /// <param name="image"></param>
    /// <param name="color"></param>
    /// <returns></returns>
    public static unsafe WriteableBitmap DrawSelectedProvince(WriteableBitmap image, uint color)
    {
        image.Lock();
        var pixels = (uint*)image.BackBuffer;
        var pixelCount = image.PixelWidth * image.PixelHeight;

        Parallel.For(0, pixelCount, (index) =>
        {
            if (pixels == null)
                return;

            var rawPixel = pixels[index];

            if (rawPixel == color) pixels[index] = GetRawColor(Colors.WhiteSmoke);
            else pixels[index] = 0;
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
    public unsafe WriteableBitmap RefreshProvincePolitical(WriteableBitmap provinceMap,
        WriteableBitmap overWrittenMap, uint provinceColor)
    {
        provinceMap.Lock();
        overWrittenMap.Lock();
        var givenPixels = (uint*)provinceMap.BackBuffer;
        var overWrittenPixels = (uint*)overWrittenMap.BackBuffer;
        var pixelCount = provinceMap.PixelWidth * provinceMap.PixelHeight;

        ProvinceData.GetColorsToProvinceIDs().TryGetValue(provinceColor, out var provinceID);
        ProvinceFile province = ProvinceData.GetProvince((uint)provinceID);

        Color countryColor = Colors.Black;

        if (province.Owner != null)
        {
            ModData.COUNTRY_DATA.GetCountries().TryGetValue(province.Owner, out Country country);
            if (country != null)
                countryColor = country.GetColor();
        }

        var newColor = GetRawColor(countryColor);

        Parallel.For(0, pixelCount, (index) =>
        {
            if (givenPixels == null)
                return;

            var rawPixel = givenPixels[index];
            if (rawPixel != provinceColor)
                return;

            if (overWrittenPixels != null)
                overWrittenPixels[index] = newColor;
        });

        provinceMap.Unlock();
        overWrittenMap.Unlock();

        return overWrittenMap;
    }
}