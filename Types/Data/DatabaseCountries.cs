using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Paradox_Editor.Types.Data;

public class DatabaseCountries
{
    // INTERNAL
    private readonly ConcurrentDictionary<Tag, uint> TagToIndex = new();

    // META DATA
    private bool[] Vanilla = [];
    public string[] HistoryFile = [];
    public string[] CommonFile = [];

    // COMMON DATA
    public string[] Tags = [];
    public string[] Name = [];
    public uint[] CountryColor = [];
    private string[] GraphicalCulture = [];

    // HISTORY DATA
    private int[] Capital = []; // Province ID.
    private string[] PrimaryCulture = [];
    private List<string>[] AcceptedCultures = [];
    private string[] Religion = [];
    private string[] Government = []; //Make a governments type
    private double[] Plurality = [];
    private string[] National_Value = []; //Make national-value type
    private double[] Literacy = []; //Decimal so that values > 0.1 are available
    private int[] Consciousness = [];
    private string[] Civilized = [];
    private string[] IsReleasableVassal = [];
    private int[] Prestige = [];
    private string[] TAG_oob = []; //For the TAG_oob.txt; not all countries have them
    private List<string>[] CountryFlags = [];
    private int[] Non_State_Consciousness = [];
    private double[] Non_State_Culture_Literacy = [];
    private string[] RulingParty = []; // Ruling Party & Upper House
    private string[] LastElection = []; // TODO: May need to have a custom type as a Date.
    private string[] TechSchools = [];
    private List<Party>[] Parties = []; // mark starting upper house. // Common
    private Dictionary<string, List<string>>[] UnitNames = []; // mark starting upper house.
    private Dictionary<string, int>[] UpperHouse = [];
    private Dictionary<string, bool>[] Inventions = [];
    private Dictionary<string, bool>[] Technologies = [];

    // Tracing to Provinces
    public Dictionary<Tag, HashSet<uint>> ProvincesByOwnedCountry = [];
    
    #region Get Methods

    public uint GetColor(Tag owner) => CountryColor[TagToIndex[owner]];

    public string GetCommonFile(string tag) => CommonFile[TagToIndex[tag]];

    public string GetHistoryFile(Tag tag) => HistoryFile[TagToIndex[tag]];
    
    #endregion

    #region Set Methods

    public void SetColor(string tag, uint color) => CountryColor[TagToIndex[tag]] = color;

    public void SetCapital(Tag tag, int capitalId) => Capital[TagToIndex[tag]] = capitalId;

    public void SetPrimaryCulture(Tag tag, string culture) => PrimaryCulture[TagToIndex[tag]] = culture;

    public void AddCulture(Tag tag, string culture) => AcceptedCultures[TagToIndex[tag]].Add(culture);

    public void SetReligion(Tag tag, string religion) => Religion[TagToIndex[tag]] = religion;

    public void SetGovernment(Tag tag, string government) => Government[TagToIndex[tag]] = government;

    public void SetPlurality(Tag tag, double plurality) => Plurality[TagToIndex[tag]] = plurality;

    public void SetNational_Value(Tag tag, string nationalValue) => National_Value[TagToIndex[tag]] = nationalValue;

    public void SetLiteracy(Tag tag, double literacy) => Literacy[TagToIndex[tag]] = literacy;

    public void SetNon_State_Culture_Literacy(Tag tag, double literacy)
        => Non_State_Culture_Literacy[TagToIndex[tag]] = literacy;

    public void SetNon_State_Culture_Consciousness(Tag tag, int consciousness)
        => Non_State_Consciousness[TagToIndex[tag]] = consciousness;

    public void SetGraphicalCulture(string tag, string readUntilWhitespace)
        => GraphicalCulture[TagToIndex[tag]] = readUntilWhitespace;

    public void SetCivilized(Tag tag, string isCivilizedText) => Civilized[TagToIndex[tag]] = isCivilizedText;

    public void SetIsReleasableVassal(Tag tag, string isVassalText) =>
        IsReleasableVassal[TagToIndex[tag]] = isVassalText;

    public void SetPrestige(Tag tag, int prestige) => Prestige[TagToIndex[tag]] = prestige;

    public void AddCountryFlag(Tag tag, string flag)
    {
        ref var flags = ref CountryFlags[TagToIndex[tag]];
        if (!flags.Contains(flag))
            flags.Add(flag);
        else Console.WriteLine($@"Duplicate country flag \'{flag}\' found for \'{tag}\'!");
    }

    public void AddParty(Tag tag, Party readParty)
    {
        // Only unique parties allowed!
        var parties = Parties[TagToIndex[tag]];
        
        
        if (!parties.Any(p => p.Name.Equals(readParty.Name)))
            Parties[TagToIndex[tag]].Add(readParty);
        else Console.WriteLine($@"Duplicate party '{readParty.Name}' found for '{tag}'!");
    }

    public void SetUnitNames(Tag tag, UnitNames readUnitNames) => UnitNames[TagToIndex[tag]] = readUnitNames;

    public void SetUpperHouse(Tag tag, Dictionary<string, int> upperHouse) => UpperHouse[TagToIndex[tag]] = upperHouse;

    public void AddOOB(Tag tag, string oobFile) => TAG_oob[TagToIndex[tag]] = oobFile;

    public void SetTechSchool(Tag tag, string school) => TechSchools[TagToIndex[tag]] = school;

    public void SetInventions(Tag tag, Dictionary<string, bool> inventions) => Inventions[TagToIndex[tag]] = inventions;

    public void SetTechnologies(Tag tag, Dictionary<string, bool> technologies)
        => Technologies[TagToIndex[tag]] = technologies;

    #endregion

    #region Utility

    public bool Contains(Tag tag) => !string.IsNullOrEmpty(tag) && Tags.Any(d => d.Equals(tag));

    private uint countryCounter = 0;

    private readonly object _countryLock = new();

    public void CreateCountry(Tag tag, string name, string commonFilePath, string historyFilePath, bool isVanilla)
    {
        if (Tags.Contains(tag.ToString()))
        {
            MessageBox.Show($"Duplicate country tag \'{tag}\' found in Common File!");
            return;
        }

        // Ensure there's just enough space.
        Expand(Tags.Length + 1);
        Tags[^1] = tag;
        Name[^1] = name;
        CommonFile[^1] = commonFilePath;
        HistoryFile[^1] = historyFilePath;
        Vanilla[^1] = isVanilla;

        lock (_countryLock)
        {
            TagToIndex[tag] = countryCounter;
            countryCounter++;
        }
    }

    private void Expand(int size)
    {
        Array.Resize(ref Vanilla, size);
        Array.Resize(ref CommonFile, size);
        Array.Resize(ref HistoryFile, size);
        Array.Resize(ref Tags, size);
        Array.Resize(ref Name, size);
        Array.Resize(ref CountryColor, size);
        Array.Resize(ref GraphicalCulture, size);
        Array.Resize(ref Capital, size);
        Array.Resize(ref PrimaryCulture, size);
        Array.Resize(ref AcceptedCultures, size);
        AcceptedCultures[size - 1] = [];
        Array.Resize(ref Religion, size);
        Array.Resize(ref Government, size);
        Array.Resize(ref Plurality, size);
        Array.Resize(ref National_Value, size);
        Array.Resize(ref Literacy, size);
        Array.Resize(ref Consciousness, size);
        Array.Resize(ref Civilized, size);
        Array.Resize(ref IsReleasableVassal, size);
        Array.Resize(ref Prestige, size);
        Array.Resize(ref TAG_oob, size);
        Array.Resize(ref CountryFlags, size);
        CountryFlags[size - 1] = [];
        Array.Resize(ref RulingParty, size);
        Array.Resize(ref LastElection, size);
        Array.Resize(ref Non_State_Culture_Literacy, size);
        Array.Resize(ref Non_State_Consciousness, size);
        Array.Resize(ref Parties, size);
        Parties[size - 1] = [];
        Array.Resize(ref UnitNames, size);
        UnitNames[size - 1] = [];
        Array.Resize(ref UpperHouse, size);
        UpperHouse[size - 1] = [];
        Array.Resize(ref TechSchools, size);
        Array.Resize(ref Inventions, size);
        Inventions[size - 1] = [];
        Array.Resize(ref Technologies, size);
        Technologies[size - 1] = [];
    }


    public void Clear()
    {
        // IMPL: Do this.
    }

    #endregion

}