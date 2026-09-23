
using System;
using System.Collections.Generic;

namespace Paradox_Editor.Types.Data;

public class ProvinceHistoryEvent
{
    public DateTime Date { get; set; }

    public string? Owner { get; set; }
    public string? Controller { get; set; }
    public string? TradeGoods { get; set; }

    public short? LifeRating { get; set; }
    public short? Colonial { get; set; }

    public string? Terrain { get; set; }

    public List<Tag> AddedCores { get; } = [];

    public short? NavalBase { get; set; }
    public short? Fort { get; set; }
    public short? Railroad { get; set; }

    public List<StateBuilding> StateBuildings { get; } = [];

    public ProvinceHistoryEvent()
    {
    }
}