using System;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Paradox_Editor.Types;

// ReSharper disable MemberCanBeMadeStatic.Global

namespace Paradox_Editor.Handlers;

public class MapRenderer
{
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

        DatabaseProvinces databaseProvinces = ModData.Instance.DatabaseProvinces;
        DatabaseCountries databaseCountries = ModData.Instance.DatabaseCountries;
        Parallel.For(0, pixelCount, (index) =>
        {
            if (pixels == null)
                return; // No null pointer exceptions, today!

            var rawPixel = pixels[index];

            //Below being simplified
            databaseProvinces.ColorsToProvinceIDs.TryGetValue(rawPixel, out var provinceID);
            databaseProvinces.TryGetOwner(provinceID, out var owner);
            if (!string.IsNullOrEmpty(owner) && databaseCountries.HasCountry(owner))
            {
                // Some tags are problematic; see Heirs to Aquitania.
                // TEMP: May be related to the lack of a Fallback for country definitions that rely on vanilla.
                pixels[index] = GetRawColor(databaseCountries.GetColor(owner));
            }
            else
            {
                // Checking if province exists. Ocean provinces are not added to the province list.
                if (databaseProvinces.IsValidLandProvince(provinceID))
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

        DatabaseProvinces databaseProvinces = ModData.Instance.DatabaseProvinces;
        var provinceID = databaseProvinces.GetIDFromColor(provinceColor);

        Color countryColor = Colors.Black;

        DatabaseCountries database = ModData.Instance.DatabaseCountries;
        if (databaseProvinces.TryGetOwner(provinceID, out var owner))
        {
            if (database.HasCountry(owner))
                countryColor = database.GetColor(owner);
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