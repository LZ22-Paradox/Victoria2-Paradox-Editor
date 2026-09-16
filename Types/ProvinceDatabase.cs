using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Windows.Media;
using Paradox_Editor.Extensions;
using Paradox_Editor.Parsers;

namespace Paradox_Editor.Types;

public class ProvinceDatabase
{
    /// Indexed Province Color -> Province ID 
    public readonly Dictionary<uint, uint> ColorsToProvinceIDs = new();

    // FILE I/O
    public readonly List<uint> ProvinceID = []; //✓
    public string[] ProvinceName = [];
    public string[] HistoryFilePath = []; //✓
    public string[] ProvinceFileName = []; //✓
    public bool[] IsOcean = [];

    /// IO Needed for CSV Processing (ESPECIALLY DO NOT TOUCH!)
    public uint[] Color = [];

    // HISTORY DATA
    public string[] Owner = [];
    public string[] Controller = [];
    public List<Tag>[] Cores = [];
    public string[] TradeGoods = [];
    public int[] LifeRating = [];
    public string[] Terrain = [];
    public int[] Colonial = [];
    public List<StateBuilding>[] State_Buildings = [];
    public int[] Naval_Base = [];
    public int[] Fort = [];
    public int[] Railroad = [];

    public static ProvinceDatabase Instance = null!;
    private readonly int ProvinceCount;

    public ProvinceDatabase(int size)
    {
        ProvinceCount = size;
        Expand(size);
        Instance = this;
    }

    /// <param name="size">Province Maximum Size obtained by the default.map</param>
    private void Expand(int size)
    {
        Array.Resize(ref ProvinceName, size);
        Array.Resize(ref HistoryFilePath, size);
        Array.Resize(ref ProvinceFileName, size);
        Array.Resize(ref Color, size);
        Array.Resize(ref Owner, size);
        Array.Resize(ref Controller, size);
        Array.Resize(ref Cores, size);
        Array.Resize(ref TradeGoods, size);
        Array.Resize(ref LifeRating, size);
        Array.Resize(ref Terrain, size);
        Array.Resize(ref Colonial, size);
        Array.Resize(ref State_Buildings, size);
        Array.Resize(ref Naval_Base, size);
        Array.Resize(ref Fort, size);
        Array.Resize(ref Railroad, size);
        Array.Resize(ref IsOcean, size);
    }

    #region Get Methods

    public uint GetColor(uint provinceId) => Color[provinceId];

    public uint GetIDFromColor(uint color) => ColorsToProvinceIDs[color];
    public uint GetIDFromColor(Color color) => GetIDFromColor(color.ToPackedColor());

    public string GetHistoryFile(uint provinceId) => HistoryFilePath[provinceId];

    public bool TryGetOwner(uint provinceId, [NotNullWhen(true)] out string? owner)
    {
        owner = Owner[provinceId];
        return !string.IsNullOrEmpty(owner);
    }

    public bool TryGetController(uint provinceId, [NotNullWhen(true)] out string? controller)
    {
        controller = Controller[provinceId];
        return !string.IsNullOrEmpty(controller);
    }

    private bool IsValidProvince(uint provinceId) => ProvinceID.Contains(provinceId);

    public bool IsValidLandProvince(uint provinceId) => IsValidProvince(provinceId) && !IsOcean[provinceId];

    public bool IsOceanProvince(uint provinceId) => ProvinceID.Contains(provinceId) && IsOcean[provinceId];

    public string GetName(uint provinceId) => ProvinceName[provinceId];

    public string GetTradeGood(uint provinceId) => TradeGoods[provinceId];

    public int GetLifeRating(uint provinceId) => LifeRating[provinceId];

    public int GetColonial(uint provinceId) => Colonial[provinceId];

    public int GetNavalBaseLevel(uint provinceId) => Naval_Base[provinceId];

    public string GetTerrain(uint provinceId) => Terrain[provinceId];

    public int GetFortLevel(uint provinceId) => Fort[provinceId];

    public int GetRailroadLevel(uint provinceId) => Railroad[provinceId];

    public List<Tag> GetCores(uint provinceId) => Cores[provinceId];

    public IEnumerable<StateBuilding> GetStateBuildings(uint provinceId) => State_Buildings[provinceId];

    public IEnumerable<ProvinceWrapper> GetLandProvinceWrappers()
    {
        // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
        foreach (var id in ProvinceID)
        {
            // Ignore Ocean provinces.
            if (IsValidLandProvince(id))
                continue;

            var wrapper = new ProvinceWrapper(id.ToString(), HistoryFilePath[id]);
            yield return wrapper;
        }
    }

    #endregion

    #region Set Methods

    public void SetOwner(uint provinceId, string tag = "") => Owner[provinceId] = tag;

    public void SetController(uint provinceId, string tag = "") => Controller[provinceId] = tag;

    public void SetTradeGood(uint provinceId, string tradeGoodBoxText) => TradeGoods[provinceId] = tradeGoodBoxText;

    public void SetLifeRating(uint provinceId, short rating) => LifeRating[provinceId] = rating;

    public void SetColonial(uint provinceId, short colonial) => Colonial[provinceId] = colonial;

    public void ClearCores(uint provinceId) => Cores[provinceId].Clear();

    public void AddCore(uint provinceId, string tag)
    {
        // Avoid duplicate cores.
        if (!Cores[provinceId].Contains(tag))
            Cores[provinceId].Add(tag);
    }

    public void SetTerrain(uint provinceId, string terrain) => Terrain[provinceId] = terrain;
    public void SetNavalBaseLevel(uint provinceId, int level) => Naval_Base[provinceId] = level;

    public void SetFortLevel(uint provinceId, int level) => Fort[provinceId] = level;

    public void SetRailroadLevel(uint provinceId, int level) => Railroad[provinceId] = level;

    public void ClearBuildings(uint provinceId) => State_Buildings[provinceId].Clear();

    public void AddStateBuilding(uint provinceId, StateBuilding building)
    {
        // Avoid duplicate buildings.
        if (!State_Buildings[provinceId].Contains(building))
            State_Buildings[provinceId].Add(building);
    }

    public void SetFileData(uint provinceId, string historyFilePath, string provinceName)
    {
        // Avoid duplicates.
        if (!ProvinceID.Contains(provinceId))
            ProvinceID.Add(provinceId);
        else throw new Exception($"Duplicate province ID found: {provinceId}");

        HistoryFilePath[provinceId] = historyFilePath;

        // WARN: TODO: Fix for the case where the name is only a number.
        ProvinceFileName[provinceId] = provinceName;

        // With the history file path now acquired, simply populate the history data.
        using var historyFiller = new HistoryFiller();
        historyFiller.PopulateHistoryData(provinceId, historyFilePath, this);
    }

    /// Populate province data with CSV Info
    public void SetCSVData(uint id, uint red, uint green, uint blue, string recordName)
    {
        // ERR: Faulty. Some provinces bug. (Tested Vanilla Vic2)
        ProvinceID.Add(id);
        uint packedColor = (0xFFu << 24) | ((red & 0xFF) << 16) | ((green & 0xFF) << 8) | (blue & 0xFF);
        Color[id] = packedColor;
        ColorsToProvinceIDs.Add(packedColor, id);
        ProvinceName[id] = recordName;
        // Debug.WriteLine("(RGB: {0}, ID: {1}, NAME: {2})", color, record.province, record.name);
    }

    #endregion
}