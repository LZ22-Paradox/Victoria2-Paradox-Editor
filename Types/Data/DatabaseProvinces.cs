using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows.Media;
using Paradox_Editor.Extensions;
using Paradox_Editor.Parsers;

namespace Paradox_Editor.Types.Data;

public class DatabaseProvinces
{
    /// Indexed Province Color -> Province ID 
    public readonly ConcurrentDictionary<uint, uint> ColorsToProvinceIDs = new();

    // FILE I/O
    private readonly uint[] ProvinceID;
    private string[] ProvinceName = [];
    private string[] HistoryFilePath = [];
    private string[] ProvinceFileName = [];
    private bool[] IsOcean = [];
    private uint[] Color = []; // Needed for CSV Processing (ESPECIALLY DO NOT TOUCH!)

    // COMMON DATA
    private List<Pop>[] Pops = []; // TODO: Store pop data, here.

    // HISTORY DATA
    private string[] Owner = [];
    private string[] Controller = [];
    private List<Tag>[] Cores = [];
    private string[] TradeGoods = [];
    private int[] LifeRating = [];
    private string[] Terrain = [];
    private int[] Colonial = [];
    private List<StateBuilding>[] State_Buildings = [];
    private int[] Naval_Base = [];
    private int[] Fort = [];
    private int[] Railroad = [];

    // ReSharper disable once NotAccessedField.Local
    private readonly int ProvinceCount;

    public DatabaseProvinces(int size)
    {
        ProvinceID = new uint[size];
        ProvinceCount = size;
        Expand(size);
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
        Array.Resize(ref Pops, size);
    }

    #region Get Methods

    public uint GetColor(uint provinceId) => Color[provinceId];

    public uint GetIDFromColor(uint color) => ColorsToProvinceIDs[color];
    public uint GetIDFromColor(Color color) => GetIDFromColor(color.ToPackedColor());

    public bool TryGetIDFromColor(uint color, [NotNullWhen(true)] out uint? id)
    {
        id = null;
        if (!ColorsToProvinceIDs.Keys.Contains(color))
            return false;

        id = ColorsToProvinceIDs[color];
        return true;
    }

    public bool TryGetIDFromColor(Color color, [NotNullWhen(true)] out uint? id)
        => TryGetIDFromColor(color.ToPackedColor(), out id);

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

    // ReSharper disable once UnusedMember.Global
    public bool IsOceanProvince(uint provinceId) => IsOcean[provinceId];

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

            var wrapper = new ProvinceWrapper(id.ToString(), ProvinceName[id], HistoryFilePath[id]);
            yield return wrapper;
        }
    }

    public List<Pop> GetPops(uint provinceId)
    {
        throw new NotImplementedException();
    }

    #endregion

    #region Set Methods

    public void SetOwner(uint provinceId, string tag = "") => Owner[provinceId] = tag;

    public void SetController(uint provinceId, string tag = "") => Controller[provinceId] = tag;

    public void SetTradeGood(uint provinceId, string tradeGoodBoxText) => TradeGoods[provinceId] = tradeGoodBoxText;

    public void SetLifeRating(uint provinceId, short rating) => LifeRating[provinceId] = rating;

    public void SetColonial(uint provinceId, short colonial) => Colonial[provinceId] = colonial;

    public void SetOcean(uint provinceId, bool value) => IsOcean[provinceId] = value;

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

    public void SetHistoryData(uint provinceId, string historyFilePath, string provinceName)
    {
        HistoryFilePath[provinceId] = historyFilePath;

        // WARN: TODO: Fix for the case where the name is only a number.
        ProvinceFileName[provinceId] = provinceName;

        // With the history file path now acquired, simply populate the history data.
        using var historyFiller = new HistoryFiller();
        historyFiller.PopulateHistoryData(provinceId, historyFilePath, this);
    }

    /// Populate province data with CSV Info
    public void CreateProvince(uint id, uint red, uint green, uint blue, string recordName, bool[] isOceanProvinceArray)
    {
        // Avoid duplicates.
        if (ProvinceID.Contains(id))
            throw new Exception($"Duplicate province ID found: {id}");

        // ERR: Faulty. Some provinces bug. (Tested Vanilla Vic2)
        ProvinceID[id] = id;
        uint packedColor = (0xFFu << 24) | ((red & 0xFF) << 16) | ((green & 0xFF) << 8) | (blue & 0xFF);
        Color[id] = packedColor;
        ColorsToProvinceIDs[packedColor] = id;
        ProvinceName[id] = recordName;

        // Ensure the arrays with internal lists are instantiated in memory to avoid any pesky null pointer
        //  exceptions. :)
        Cores[id] = [];
        Pops[id] = [];
        State_Buildings[id] = [];
        IsOcean = isOceanProvinceArray;

        // Debug.WriteLine("(RGB: {0}, ID: {1}, NAME: {2})", color, record.province, record.name);
    }

    public void SetCores(uint id, List<Tag> tempCoresList) => Cores[id] = tempCoresList;

    public void SetPops(uint provinceId)
    {
        throw new NotImplementedException();
    }

    public void AddPops(uint provinceId)
    {
        throw new NotImplementedException();
    }

    #endregion
}