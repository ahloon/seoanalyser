using Microsoft.AspNetCore.Mvc;
using SeoAnalyser.Models;
using SeoAnalyser.Services;
using System.Collections.Generic;

namespace SeoAnalyser.Controllers
{
	[Route("api/[controller]")]
	public class SampleDataController : Controller
	{
		[HttpGet("[action]/{text}/{isUrl}/{inPage}/{inMeta}/{external}")]
		public IEnumerable<Result> AnalyserResults(string text, bool isUrl, bool inPage, bool inMeta, bool external)
		{
			Analyser seoAnalyser = new Analyser(text)
			{
				IsUrl = isUrl,
				OccurencesInPage = inPage,
				OccurencesInMetaTag = inMeta,
				ExternalLinks = external

			};
			return seoAnalyser.Analyse();
		}
	}
}
