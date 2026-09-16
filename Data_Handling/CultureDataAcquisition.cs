using Paradox_Editor.Parsers;
using System.Collections.Generic;

namespace Paradox_Editor.Data_Handling
{
    public class CultureDataAcquisition
	{
		private readonly List<string> cultures = new();

		public CultureDataAcquisition(string directory)
		{
			var cultureGroups = CultureParser.Parse(directory);
			foreach (var cultureGroup in cultureGroups)
			foreach (var culture in cultureGroup.Value.Cultures.Keys)
			{
				cultures.Add(culture);
			}
			cultures.Sort();
		}

		public List<string> GetCultures() => cultures;
	}
}
