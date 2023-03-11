using Paradox_Editor.Cultures;
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

		public MapFileDataAcquisition(string directory)
		{
			continents = ContinentParser.Parse(directory);
		}

		public Dictionary<string, Continent> GetContinents() => continents;
	}
}
