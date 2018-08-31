using Microsoft.VisualStudio.TestTools.UnitTesting;
using SeoAnalyser.Models;
using SeoAnalyser.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SeoAnalyserTest
{
	[TestClass]
	public class UnitTest1
	{
		ISeoAnalyser seoAnalyser;
		List<Result> results;

		[TestMethod, ExpectedException(typeof(ArgumentNullException), "Textbox is not expected to be empty.")]
		public void Analyser_SubmitsEmptyText_ReturnsError()
		{
			//arrange
			seoAnalyser = new Analyser(" ");

			//act
			results = seoAnalyser.Analyse();
		}

		[TestMethod]
		public void Analyser_SubmitsTextWithoutCheckBoxes_ReturnsListOfWordsAndOccurences()
		{
			//arrange
			seoAnalyser = new Analyser("Kuala Lumpur is a big city.");

			//act
			results = seoAnalyser.Analyse();

			//assert
			List<Result> expectedText = new List<Result>() { new Result() { Word = "Kuala", Occurrences = 0 },
															 new Result() { Word = "Lumpur", Occurrences = 0 },
															 new Result() { Word = "big", Occurrences = 0 },
															 new Result() { Word = "city", Occurrences = 0 } };

			Assert.AreEqual(4, results.Except(expectedText).Count());
			foreach (Result text in results)
			{
				Assert.IsTrue(text.Occurrences == 0);
			}
		}

		[TestMethod]
		public void Analyser_SubmitsTextWithSpecialCharacters_ReturnsListOfWordsAndOccurences()
		{
			//arrange
			seoAnalyser = new Analyser("Kuala Lumpur / is a *& big city.")
			{
				OccurencesInPage = true
			};

			//act
			results = seoAnalyser.Analyse();

			//assert
			List<Result> expectedText = new List<Result>() { new Result() { Word = "Kuala", Occurrences = 1 },
															 new Result() { Word = "Lumpur", Occurrences = 1 },
															 new Result() { Word = "big", Occurrences = 1 },
															 new Result() { Word = "city", Occurrences = 1 } };
			Assert.AreEqual(4, results.Except(expectedText).Count());
			foreach (Result text in results)
			{
				Assert.IsTrue(text.Occurrences == 1);
			}
		}

		[TestMethod]
		public void Analyser_SubmitsTextWithExternalLinks_ReturnsListOfWordsAndOccurences()
		{
			//arrange
			seoAnalyser = new Analyser("http://www.google.com http://www.google.com")
			{
				ExternalLinks = true,
				OccurencesInPage = true
			};

			//act
			results = seoAnalyser.Analyse();

			//assert
			Assert.AreEqual("ExternalLink", results[0].Word);
			Assert.AreEqual(2, results[0].Occurrences);
		}

		[TestMethod, ExpectedException(typeof(System.Net.WebException), "Invalid URL should not be possible.")]
		public void Analyser_SubmitInvalidUrlOnly_ReturnsInvalidUrlError()
		{
			//arrange
			seoAnalyser = new Analyser("http://www.google1.com")
			{
				IsUrl = true
			};

			//act
			results = seoAnalyser.Analyse();

			//assert
		}

		[TestMethod]
		public void Analyser_SubmitHtmlOnly_ReturnsListOfWordsAndOccurencesInMetatag()
		{
			//arrange
			seoAnalyser = new Analyser(@"<!DOCTYPE html>
					< html lang = 'en' xmlns = 'http://www.w3.org/1999/xhtml' >
					< head >
					< meta charset = 'utf-8' />
					< meta content = 'Test' />
					< meta content = 'Test' />
					</ head >
					< body >
					This is just a test on meta tags.
					</ body >
					</ html >")
			{
				OccurencesInMetaTag = true,
				OccurencesInPage = true

			};

			//act
			results = seoAnalyser.Analyse();

			//assert
			Assert.AreEqual(3, results.Single(result => result.Word.ToLower() == "test").Occurrences);
		}
	}
}
