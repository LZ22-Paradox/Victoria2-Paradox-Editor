using Paradox_Editor.Parsers;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;

namespace Paradox_Editor.Data_Handling
{
    public class MapFileDataAcquisition
	{
		private readonly Dictionary<string, Continent> _continents;
		private readonly Dictionary<string, GoodGroup> _goods;

		public MapFileDataAcquisition(string directory)
		{
			_continents = ContinentParser.Parse(directory);
			_goods = GoodsParser.Parse(directory);
		}

		public ImmutableDictionary<string, Continent> GetContinents() => _continents.ToImmutableDictionary();
		public ImmutableDictionary<string, GoodGroup> GetGoods() => _goods.ToImmutableDictionary();
	}
}
