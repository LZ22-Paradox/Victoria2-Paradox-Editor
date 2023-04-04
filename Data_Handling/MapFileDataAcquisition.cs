using Paradox_Editor.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paradox_Editor.Data_Handling
{
    public class MapFileDataAcquisition
	{
		private Dictionary<string, Continent> continents;
		private Dictionary<string, GoodGroup> goods;

		public MapFileDataAcquisition(string directory)
		{
			continents = ContinentParser.Parse(directory);
			goods = GoodsParser.Parse(directory);
		}

		public Dictionary<string, Continent> GetContinents() => continents;
		public Dictionary<string, GoodGroup> GetGoods() => goods;
	}
}
