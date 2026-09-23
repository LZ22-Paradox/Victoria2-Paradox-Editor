using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Paradox_Editor.ColorPickerControls;
using Paradox_Editor.Extensions;
using Paradox_Editor.Types.Data;

// ReSharper disable MemberCanBeMadeStatic.Global

namespace Paradox_Editor.Handlers;

public static class MapRenderer
{
    private const uint WHITE = 4294967295; // White
    private const uint WHITE_SMOKE = 4294309365; // White
    private const uint BLACK = 4278190080; // Black
    private const uint DARK_GRAY = 4289309097; // Black

    public static void CreateStateMap()
    {
        // States, aka "regions" are found in ...mod/map/region.txt whilst continent provinces are located in continent.txt
        throw new NotImplementedException();
    }

    public static void CreateCultureMap()
    {
        throw new NotImplementedException();
    }

    public static void CreatePopulationMap()
    {
        throw new NotImplementedException();
    }

    public static unsafe WriteableBitmap CreatePoliticalMap(WriteableBitmap provinceMap)
    {
        var politicalMap = new WriteableBitmap(
            provinceMap.PixelWidth,
            provinceMap.PixelHeight,
            provinceMap.DpiX,
            provinceMap.DpiY,
            PixelFormats.Pbgra32,
            null);

        provinceMap.Lock();
        politicalMap.Lock();

        try
        {
            var sourcePixels = (uint*)provinceMap.BackBuffer;
            var resultPixels = (uint*)politicalMap.BackBuffer;
            int pixelCount = provinceMap.PixelWidth * provinceMap.PixelHeight;

            Parallel.For(0, pixelCount, index => resultPixels[index] = GetPoliticalColor(sourcePixels[index]));

            politicalMap.AddDirtyRect(new Int32Rect(0, 0, politicalMap.PixelWidth, politicalMap.PixelHeight));
        }
        finally
        {
            politicalMap.Unlock();
            provinceMap.Unlock();
        }

        return politicalMap;
    }

    /// <summary>
    /// Refreshes all pixels belonging to a single province in the political map.
    /// </summary>
    public static unsafe WriteableBitmap RefreshProvincePolitical(
        WriteableBitmap provinceMap,
        WriteableBitmap overWrittenMap,
        uint provinceColor)
    {
        provinceMap.Lock();
        overWrittenMap.Lock();

        try
        {
            var sourcePixels = (uint*)provinceMap.BackBuffer;
            var resultPixels = (uint*)overWrittenMap.BackBuffer;
            int pixelCount = provinceMap.PixelWidth * provinceMap.PixelHeight;
            uint politicalColor = GetPoliticalColor(provinceColor);

            Parallel.For(0, pixelCount, index =>
            {
                if (sourcePixels[index] == provinceColor)
                    resultPixels[index] = politicalColor;
            });

            overWrittenMap.AddDirtyRect(new Int32Rect(0, 0, overWrittenMap.PixelWidth, overWrittenMap.PixelHeight));
        }
        finally
        {
            overWrittenMap.Unlock();
            provinceMap.Unlock();
        }

        return overWrittenMap;
    }

    #region Utility

    /// <summary>
    /// Converts a province-map color into its political-map color.
    /// </summary>
    private static uint GetPoliticalColor(uint provinceColor)
    {
        DatabaseProvinces provinces = ModData.Instance.DatabaseProvinces;
        DatabaseCountries countries = ModData.Instance.DatabaseCountries;

        // Unknown province.
        if (!provinces.ColorsToProvinceIDs.TryGetValue(provinceColor, out var provinceId))
            return BLACK;

        // Ocean is deliberately white.
        if (provinces.IsOceanProvince(provinceId))
            return WHITE;

        // Land without an owner.
        if (!provinces.TryGetOwner(provinceId, out var owner))
            return DARK_GRAY;

        // Owner exists, but isn't present in the country database.
        // ReSharper disable once ConvertIfStatementToReturnStatement
        if (!countries.Contains(owner))
            return BLACK;

        return countries.GetColor(owner);
    }

    public static uint GetColorAt(WriteableBitmap bmp, ImageColorPicker map)
        => GetProvinceColorAt(bmp, map.Position, new Size(map.ActualWidth, map.ActualHeight)).ToPackedColor();

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
                return ((uint*)provinceMap.BackBuffer)[y * provinceMap.PixelWidth + x].UnpackAsArgbColor();
            }
        }
        finally
        {
            provinceMap.Unlock();
        }
    }

    #endregion

    public static unsafe WriteableBitmap DrawProvinces(WriteableBitmap provinceMap, IEnumerable<uint> provinceIds)
    {
        var result = new WriteableBitmap(
            provinceMap.PixelWidth,
            provinceMap.PixelHeight,
            provinceMap.DpiX,
            provinceMap.DpiY,
            PixelFormats.Pbgra32,
            null);

        DatabaseProvinces provinces = ModData.Instance.DatabaseProvinces;
        var selectedProvinceIds = provinceIds.ToHashSet();

        provinceMap.Lock();
        result.Lock();

        try
        {
            var sourcePixels = (uint*)provinceMap.BackBuffer;
            var resultPixels = (uint*)result.BackBuffer;

            int pixelCount =
                provinceMap.PixelWidth *
                provinceMap.PixelHeight;

            Parallel.For(0, pixelCount, index =>
            {
                uint provinceColor = sourcePixels[index];

                // Only draw pixels belonging to the selected provinces.
                if (provinces.ColorsToProvinceIDs.TryGetValue(
                        provinceColor,
                        out uint provinceId) &&
                    selectedProvinceIds.Contains(provinceId))
                {
                    resultPixels[index] = WHITE;
                }
                else
                {
                    // Completely transparent.
                    resultPixels[index] = 0;
                }
            });

            result.AddDirtyRect(
                new Int32Rect(
                    0,
                    0,
                    result.PixelWidth,
                    result.PixelHeight));
        }
        finally
        {
            result.Unlock();
            provinceMap.Unlock();
        }

        return result;
    }

    #region OBSOLETE // Nostalgia.

    /// <summary>
    /// Redraws the given province that has been selected.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="color"></param>
    /// <returns></returns>
    [Obsolete("Use DrawProvinces instead!")]
    // ReSharper disable once UnusedMember.Global
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

            Parallel.For(0, pixelCount, index
                => resultPixels[index] = sourcePixels[index] == color ? WHITE_SMOKE : 0);

            result.AddDirtyRect(new Int32Rect(0, 0, result.PixelWidth, result.PixelHeight));
        }
        finally
        {
            result.Unlock();
            source.Unlock();
        }

        return result;
    }

    #endregion
}