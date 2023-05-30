using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace webcrawlerhttp
{
	public class Report
	{
		public string PrintReport(Crawl crawl)
		{
			var output = "=========\n" +
						 "REPORT\n" +
						 "=========\n";
			var sortedPages = SortPages(crawl.Pages);
			foreach (var sortedPage in sortedPages)
			{
				var url = sortedPage.Key;
				var hits = sortedPage.Value;
				output += $"Found {hits} links to page: {url}\n";
			}

			output += $"Total external links: {crawl.ExternalLinksCount}\n";
			output += $"Total internal links: {crawl.InternalLinksCount}\n";

			return output += "=========\n" +
							 "END REPORT\n" +
							 "=========\n";
		}
		
		public Dictionary<string, int> SortPages(Dictionary<string, int> pages)
		{
			var sortedURLs = pages.OrderByDescending(q => q.Value)
				.ToDictionary(q => q.Key, p => p.Value);
			return sortedURLs;
		}
	}
}
