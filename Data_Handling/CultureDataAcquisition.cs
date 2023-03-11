using Paradox_Editor.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paradox_Editor.Data_Handling
{
    public class CultureDataAcquisition
	{
		private List<string> cultures = new();

		private Dictionary<string, CultureGroup> cultureGroups = new();

		public CultureDataAcquisition(string directory)
		{
			cultureGroups = CultureParser.Parse(directory);
			foreach (var cultureGroup in cultureGroups)
			{
				foreach (var culture in cultureGroup.Value.Cultures.Keys)
				{
					cultures.Add(culture);
				}
			}
			cultures.Sort();
		}

		public List<string> GetCultures() => cultures;
	}
}
