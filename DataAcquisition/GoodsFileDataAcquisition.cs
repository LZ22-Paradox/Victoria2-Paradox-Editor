using System.Collections.Generic;
using System.Collections.Immutable;
using Paradox_Editor.Types;

namespace Paradox_Editor.DataAcquisition;

public class GoodsFileDataAcquisition
{
    private readonly Dictionary<string, GoodGroup> _goods;

    public GoodsFileDataAcquisition(string directory)
    {
        _goods = new GoodsParser().Parse<Dictionary<string, GoodGroup>>(directory, "common", "goods.txt");
    }

    public ImmutableDictionary<string, GoodGroup> GetGoods() => _goods.ToImmutableDictionary();

}