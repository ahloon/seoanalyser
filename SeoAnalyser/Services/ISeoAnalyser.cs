using SeoAnalyser.Models;
using System.Collections.Generic;

namespace SeoAnalyser.Services
{
	public interface ISeoAnalyser
	{
		string Text { get; }
		bool IsUrl { get; set; }
		bool OccurencesInPage { get; set; }
		bool OccurencesInMetaTag { get; set; }
		bool ExternalLinks { get; set; }
		List<Result> Analyse();
	}
}