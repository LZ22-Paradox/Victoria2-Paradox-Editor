using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Paradox_Editor.Types;
using Paradox_Editor.Types.Data;

// ReSharper disable MemberCanBeMadeStatic.Global

namespace Paradox_Editor.Handlers;

public class MapRenderer
{
    public static uint GetRawColor(Color color) => 0xFFu << 24 | (uint)color.R << 16 | (uint)color.G << 8 | color.B;

    #region Drawing Navigatable Maps

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
    /// <param name="source"></param>
    /// <param name="color"></param>
    /// <returns></returns>
    public static unsafe WriteableBitmap DrawConnectedColors(WriteableBitmap source, uint color)
    {
        var result = new WriteableBitmap(
            source.PixelWidth,
            source.PixelHeight,
            source.DpiX,
            source.DpiY,
            PixelFormats.Pbgra32,
            null);

        source.Lock();
        result.Lock();

        try
        {
            var sourcePixels = (uint*)source.BackBuffer;
            var resultPixels = (uint*)result.BackBuffer;

            int pixelCount = source.PixelWidth * source.PixelHeight;
            uint highlightColor = GetRawColor(Colors.WhiteSmoke);

            Parallel.For(0, pixelCount, index
                => resultPixels[index] = sourcePixels[index] == color ? highlightColor : 0);

            result.AddDirtyRect(new Int32Rect(0, 0, result.PixelWidth, result.PixelHeight));
        }
        finally
        {
            result.Unlock();
            source.Unlock();
        }

        return result;
    }

    /// <summary>
    /// Refreshes a single province in the political map. Could be rewritten in the future to accomodate for other
    /// map types.
    /// </summary>
    /// <param name="provinceMap"></param>
    /// <param name="overWrittenMap"></param>
    /// <param name="provinceColor"></param>
    /// <returns></returns>
    public unsafe WriteableBitmap RefreshProvincePolitical(
        WriteableBitmap provinceMap,
        WriteableBitmap overWrittenMap,
        uint provinceColor)
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

    public static Color GetProvinceColorAt(WriteableBitmap provinceMap, Point position, Size displaySize)
    {
        int x = Math.Clamp((int)(position.X * provinceMap.PixelWidth / displaySize.Width), 0,
            provinceMap.PixelWidth - 1);
        int y = Math.Clamp((int)(position.Y * provinceMap.PixelHeight / displaySize.Height), 0,
            provinceMap.PixelHeight - 1);

        provinceMap.Lock();

        try
        {
            unsafe
            {
                return FromRawColor(((uint*)provinceMap.BackBuffer)[y * provinceMap.PixelWidth + x]);
            }
        }
        finally
        {
            provinceMap.Unlock();
        }
    }

    private static Color FromRawColor(uint color)
    {
        return Color.FromArgb(
            (byte)(color >> 24),
            (byte)(color >> 16),
            (byte)(color >> 8),
            (byte)color);
    }

    public unsafe WriteableBitmap DrawPoliticalMap(WriteableBitmap provinceMap)
    {
        var result = new WriteableBitmap(
            provinceMap.PixelWidth,
            provinceMap.PixelHeight,
            provinceMap.DpiX,
            provinceMap.DpiY,
            PixelFormats.Pbgra32,
            null);

        DatabaseProvinces provinces = ModData.Instance.DatabaseProvinces;
        DatabaseCountries countries = ModData.Instance.DatabaseCountries;

        provinceMap.Lock();
        result.Lock();

        try
        {
            var sourcePixels = (uint*)provinceMap.BackBuffer;
            var resultPixels = (uint*)result.BackBuffer;

            int pixelCount = provinceMap.PixelWidth * provinceMap.PixelHeight;
            Parallel.For(0, pixelCount, index =>
            {
                uint provinceColor = sourcePixels[index];

                // Default to black.
                if (!provinces.ColorsToProvinceIDs.TryGetValue(provinceColor, out var provinceId))
                {
                    resultPixels[index] = GetRawColor(Colors.Black);
                    return;
                }

                // Ocean is deliberately white.
                if (provinces.IsOceanProvince(provinceId))
                {
                    resultPixels[index] = GetRawColor(Colors.White);
                    return;
                }

                // Land without an owner is black.
                if (!provinces.TryGetOwner(provinceId, out var owner) ||
                    string.IsNullOrEmpty(owner) ||
                    !countries.HasCountry(owner))
                {
                    resultPixels[index] = GetRawColor(Colors.DarkGray);
                    return;
                }

                resultPixels[index] = GetRawColor(countries.GetColor(owner));
            });

            result.AddDirtyRect(new Int32Rect(0, 0, result.PixelWidth, result.PixelHeight));
        }
        finally
        {
            result.Unlock();
            provinceMap.Unlock();
        }

        return result;
    }
}