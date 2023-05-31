using HtmlAgilityPack;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace webcrawlerhttp
{
	public class Crawl : IReportData
	{
		public Dictionary<string, int> Pages { get; set; }
		public int ExternalLinksCount { get; set; }
		public int InternalLinksCount { get; set; }
		public async Task<Crawl> CrawlPage(string baseURL, string currentURL, Crawl crawl)
		{
			var baseURLObj = new Uri(baseURL);
			var currentURLObj = new Uri(currentURL);
			if (baseURLObj.Host != currentURLObj.Host)
			{
				return crawl;
			}

			var normalizedCurrentURL = NormalizeURL(currentURL);
			if (crawl.Pages.TryGetValue(normalizedCurrentURL, out int count))
			{
				crawl.Pages[normalizedCurrentURL] = count + 1;
				return crawl;
			}

			crawl.Pages[normalizedCurrentURL] = 1;

			//MessageBox.Show($"actively crawling {currentURL}");

			try
			{
				var httpClient = new HttpClient();
				httpClient.Timeout = TimeSpan.FromSeconds(10);
				var resp = await httpClient.GetAsync(currentURL);
				if ((int)resp.StatusCode > 399)
				{
					//MessageBox.Show($"error in fetch with status code: {(int)resp.StatusCode} on page: {currentURL}");
					return crawl;
				}
				var contentType = resp.Content.Headers.GetValues("Content-Type").FirstOrDefault();
				if (!contentType.Contains("text/html"))
				{
					//MessageBox.Show($"non html response, content type: {contentType}, on page: {currentURL}");
					return crawl;
				}


				var htmlBody = await resp.Content.ReadAsStringAsync();

				var nextURLs = GetURLsFromHTML(htmlBody, baseURL);

				foreach (var nextURL in nextURLs)
				{
					if (IsInternalLink(baseURL, nextURL))
					{
						crawl.InternalLinksCount++;
					}
					else
					{
						crawl.ExternalLinksCount++;
					}
					crawl = await CrawlPage(baseURL, nextURL, crawl);

				}
			}
			catch (TaskCanceledException ex)
			{
				MessageBox.Show($"The request timed out: {ex.Message}");
			}
			catch (Exception ex)
			{
				MessageBox.Show($"error in fetch: {ex.Message}, on page {currentURL}");
			}
			return crawl;
		}

		public List<string> GetURLsFromHTML(string htmlBody, string baseURL)
		{
			var urls = new List<string>();
			var dom = new HtmlDocument();
			dom.LoadHtml(htmlBody);
			var linkElements = dom.DocumentNode.SelectNodes("//a");
			if (linkElements != null)
			{
				foreach ( var linkElement in linkElements )
				{
					string href = linkElement.GetAttributeValue("href", "");
					if (href.Substring(0, 1) == "/")
					{
						//relative
						try
						{
							var urlObj = new Uri(baseURL + href);
							urls.Add(urlObj.Scheme + "://" + urlObj.Host + urlObj.AbsolutePath);
						}
						catch (Exception err)
						{
							//MessageBox.Show($"error with relative url:\n{err.Message}");
						}
					}
					else
					{
						//absolute
						try
						{
							var urlObj = new Uri(href);
							urls.Add(urlObj.Scheme + "://" + urlObj.Host + urlObj.AbsolutePath);
						}
						catch (Exception err)
						{
							//MessageBox.Show($"error with absolute url:\n{err.Message}");
						}
					}
				}
			}
			return urls;
		}

		public string NormalizeURL(string urlString)
		{
			var urlObj = new Uri(urlString);
			var hostPath = $"{urlObj.Host}{urlObj.AbsolutePath}";
			if (hostPath.Length > 0 && hostPath.Substring(hostPath.Length - 1, 1)[0] == '/')
			{
				return hostPath.Substring(0, hostPath.Length-1);
			}
			return hostPath;
		}

		public bool IsInternalLink(string baseURL, string link)
		{
			var baseUri = new Uri(baseURL);
			var linkUri = new Uri(link);
			return baseUri.Host == linkUri.Host;
		}
	}
}
