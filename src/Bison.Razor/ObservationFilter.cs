using System.Collections.Generic;
using System.Linq;
using _module;
using Bison.Razor.DBObjects;

public static class ObservationFilter
{
	public static List<Observation> FilterBy(Taxon root, IEnumerable<Observation> observations)
	{
		var input = Dafny.Sequence<Observation>.FromArray(observations.ToArray());
		var result = __default.FilterBy(root, input);
		return result.CloneAsArray().ToList();
	}
}
