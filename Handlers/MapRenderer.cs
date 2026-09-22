using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Paradox_Editor.Types.Data;

// ReSharper disable MemberCanBeMadeStatic.Global

namespace Paradox_Editor.Handlers;

public static class MapRenderer
{
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

    public static unsafe WriteableBitmap DrawOwnedProvinces(WriteableBitmap provinceMap, string countryId)
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
            Parallel.For((long)0, pixelCount, index =>
            {
                uint provinceColor = sourcePixels[index];
                if (!provinces.ColorsToProvinceIDs.TryGetValue(provinceColor, out uint provinceId))
                {
                    resultPixels[index] = GetRawColor(Colors.Transparent);
                    return;
                }

                if (!provinces.TryGetOwner(provinceId, out var owner))
                {
                    resultPixels[index] = GetRawColor(Colors.Transparent);
                    return;
                }

                if (!owner.Equals(countryId))
                {
                    resultPixels[index] = GetRawColor(Colors.Transparent);
                    return;
                }

                resultPixels[index] = GetRawColor(Colors.White);
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
            return GetRawColor(Colors.Black);

        // Ocean is deliberately white.
        if (provinces.IsOceanProvince(provinceId))
            return GetRawColor(Colors.White);

        // Land without an owner.
        if (!provinces.TryGetOwner(provinceId, out var owner))
            return GetRawColor(Colors.DarkGray);

        // Owner exists, but isn't present in the country database.
        // ReSharper disable once ConvertIfStatementToReturnStatement
        if (!countries.Contains(owner))
            return GetRawColor(Colors.Black);

        return countries.GetColor(owner);
    }

    public static uint GetRawColor(Color color) => 0xFFu << 24 | (uint)color.R << 16 | (uint)color.G << 8 | color.B;

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

            uint highlightColor =
                GetRawColor(Colors.White);

            Parallel.For(0, pixelCount, index =>
            {
                uint provinceColor = sourcePixels[index];

                // Only draw pixels belonging to the selected provinces.
                if (provinces.ColorsToProvinceIDs.TryGetValue(
                        provinceColor,
                        out uint provinceId) &&
                    selectedProvinceIds.Contains(provinceId))
                {
                    resultPixels[index] = highlightColor;
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
}