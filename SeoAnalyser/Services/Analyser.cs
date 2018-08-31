using HtmlAgilityPack;
using SeoAnalyser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SeoAnalyser.Services
{
	public class Analyser : ISeoAnalyser
	{
		public readonly string[] StopWords = { "a", "about", "above", "after", "again", "against", "all", "am", "an",
			"and", "any", "are", "as", "at", "be", "because", "been", "before", "being", "below", "between", "both",
			"but", "by", "could", "did", "do", "does", "doing", "down", "during", "each", "few", "for", "from", "further",
			"had", "has", "have", "having", "he", "he'd", "he'll", "he's", "her", "here", "here's", "hers", "herself", "him",
			"himself", "his", "how", "how's", "i", "i'd", "i'll", "i'm", "i've", "if", "in", "into", "is", "it", "it's",
			"its", "itself", "let's", "me", "more", "most", "my", "myself", "nor", "of", "on", "once", "only", "or", "other",
			"ought", "our", "ours", "ourselves", "out", "over", "own", "same", "she", "she'd", "she'll", "she's", "should", "so",
			"some", "such", "than", "that", "that's", "the", "their", "theirs", "them", "themselves", "then", "there", "there's",
			"these", "they", "they'd", "they'll", "they're", "they've", "this", "those", "through", "to", "too", "under", "until",
			"up", "very", "was", "we", "we'd", "we'll", "we're", "we've", "were", "what", "what's", "when", "when's", "where",
			"where's", "which", "while", "who", "who's", "whom", "why", "why's", "with", "would", "you", "you'd", "you'll", "you're",
			"you've", "your", "yours", "yourself", "yourselves" };

		public string Text { get; }
		public bool IsUrl { get; set; }
		public bool OccurencesInPage { get; set; }
		public bool OccurencesInMetaTag { get; set; }
		public bool ExternalLinks { get; set; }

		public Analyser(string input)
		{
			Text = input;

		}

		public List<Result> Analyse()
		{
			if (string.IsNullOrWhiteSpace(Text))
			{
				throw new ArgumentNullException("Please enter Text or URL.");
			}


			string processedText = GetTextFromUrl();


			if (ExternalLinks)
			{
				processedText = Regex.Replace(processedText,
				@"((http|ftp|https):\/\/[\w\-_]+(\.[\w\-_]+)+([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?)",
				"ExternalLink");
			}

			var matches = Regex.Matches(processedText, @"\b[\w']*\b");
			string[] words = matches.Where(match => !string.IsNullOrEmpty(match.Value))
				.Select(m => m.Value).Where(word => !StopWords.Contains(word, StringComparer.CurrentCultureIgnoreCase)).ToArray();

			if (OccurencesInPage)
			{
				return words.GroupBy(word => word, (word, occurence) => new Result { Word = word, Occurrences = occurence.Count() }, StringComparer.CurrentCultureIgnoreCase).ToList();
			}

			return words.GroupBy(word => word, (word, occurence) => new Result { Word = word, Occurrences = 0 }, StringComparer.CurrentCultureIgnoreCase).ToList();
		}

		private string GetTextFromUrl()
		{
			try
			{
				var web = new HtmlWeb();
				var htmlDoc = new HtmlDocument();
				if (IsUrl)
				{
					htmlDoc = web.Load(System.Uri.UnescapeDataString(Text));
				}
				else
				{
					htmlDoc.LoadHtml(Text);
				}

				var sb = new StringBuilder();
				var nodes = htmlDoc.DocumentNode.Descendants().Where(n =>
					n.NodeType == HtmlNodeType.Text &&
					n.ParentNode.Name != "script" &&
					n.ParentNode.Name != "style");

				foreach (var node in nodes)
				{
					sb.AppendLine(node.InnerText.Trim());
				}

				
				return GetFormattedText(htmlDoc, sb);

			}
			catch (Exception)
			{
				throw new System.Net.WebException(Text);
			}
		}

		private string GetFormattedText(HtmlDocument htmlDoc, StringBuilder sb)
		{
			if (ExternalLinks && htmlDoc.DocumentNode.SelectNodes("//a[@href]") != null)
			{
				foreach (var link in htmlDoc.DocumentNode.SelectNodes("//a[@href]"))
				{
					var hrefValue = link.GetAttributeValue("href", string.Empty);
					if (!link.GetAttributeValue("href", string.Empty).Contains(Text))
					{
						sb.AppendLine(hrefValue);
					}
				}
			}

			if (OccurencesInMetaTag && htmlDoc.DocumentNode.SelectNodes("//meta/@content") != null)
			{
				foreach (var node in htmlDoc.DocumentNode.SelectNodes("//meta/@content"))
				{
					sb.AppendLine(node.GetAttributeValue("content", ""));
				}
			}

			return sb.ToString().Trim();
		}
	}
}
