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
	/// <summary>
	/// Represents a class that performs crawling of web pages and collects information for generating reports.
	/// </summary>
	public class Crawl : IReportData
	{
		/// <summary>
		/// Gets or sets the dictionary of pages and their hit counts.
		/// </summary>
		public Dictionary<string, int> Pages { get; set; }
		/// <summary>
		/// Gets or sets the count of external links.
		/// </summary>
		public int ExternalLinksCount { get; set; }
		/// <summary>
		/// Gets or sets the count of internal links.
		/// </summary>
		public int InternalLinksCount { get; set; }
		/// <summary>
		/// Crawls the specified web page and collects information for generating reports.
		/// </summary>
		/// <param name="baseUrl">The base URL of the website.</param>
		/// <param name="currentUrl">The URL of the current page to crawl.</param>
		/// <param name="crawl">The crawl object to store the crawling results.</param>
		/// <returns>The crawl object with updated information.</returns>
		public async Task<Crawl> CrawlPage(string baseUrl, string currentUrl, Crawl crawl)
		{
			// If this is an offsite URL, bail immediately
			var baseUrlObj = new Uri(baseUrl);
			var currentUrlObj = new Uri(currentUrl);
			if (baseUrlObj.Host != currentUrlObj.Host)
			{
				return crawl;
			}

			var normalizedCurrentUrl = NormalizeUrl(currentUrl);

			// if the page was already visited, just increase the count and don't repeat the http request
			if (crawl.Pages.TryGetValue(normalizedCurrentUrl, out var count))
			{
				crawl.Pages[normalizedCurrentUrl] = count + 1;
				return crawl;
			}

			// initialize this page in the map since it doesn't exist yet
			crawl.Pages[normalizedCurrentUrl] = 1;

			// fetch and parse the html of the currentURL
			//MessageBox.Show($"actively crawling {currentURL}");

			try
			{
				var httpClient = new HttpClient();
				httpClient.Timeout = TimeSpan.FromSeconds(10);
				var resp = await httpClient.GetAsync(currentUrl);
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

				var nextUrLs = GetUrLsFromHtml(htmlBody, baseUrl);

				foreach (var nextUrl in nextUrLs)
				{
					if (IsInternalLink(baseUrl, nextUrl))
					{
						crawl.InternalLinksCount++;
					}
					else
					{
						crawl.ExternalLinksCount++;
					}
					crawl = await CrawlPage(baseUrl, nextUrl, crawl);

				}
			}
			catch (TaskCanceledException ex)
			{
				MessageBox.Show($"The request timed out: {ex.Message}");
			}
			catch (Exception ex)
			{
				MessageBox.Show($"error in fetch: {ex.Message}, on page {currentUrl}");
			}
			return crawl;
		}

		/// <summary>
		/// Retrieves URLs from the HTML body of a web page.
		/// </summary>
		/// <param name="htmlBody">The HTML body of the web page.</param>
		/// <param name="baseUrl">The base URL of the web page.</param>
		/// <returns>A list of URLs extracted from the HTML body.</returns>
		public List<string> GetUrLsFromHtml(string htmlBody, string baseUrl)
		{
			var urls = new List<string>();
			var dom = new HtmlDocument();
			dom.LoadHtml(htmlBody);
			var linkElements = dom.DocumentNode.SelectNodes("//a");
			if (linkElements == null) return urls;
			foreach ( var linkElement in linkElements )
			{
				var href = linkElement.GetAttributeValue("href", "");
				if (href.Substring(0, 1) == "/")
				{
					//relative
					try
					{
						var urlObj = new Uri(baseUrl + href);
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
			return urls;
		}

		/// <summary>
		/// Normalizes a URL by removing trailing slashes.
		/// </summary>
		/// <param name="urlString">The URL string to normalize.</param>
		/// <returns>The normalized URL.</returns>
		public string NormalizeUrl(string urlString)
		{
			var urlObj = new Uri(urlString);
			var hostPath = $"{urlObj.Host}{urlObj.AbsolutePath}";
			if (hostPath.Length > 0 && hostPath.Substring(hostPath.Length - 1, 1)[0] == '/')
			{
				return hostPath.Substring(0, hostPath.Length-1);
			}
			return hostPath;
		}

		/// <summary>
		/// Determines whether a link is an internal link based on the base URL.
		/// </summary>
		/// <param name="baseUrl">The base URL of the website.</param>
		/// <param name="link">The link URL to check.</param>
		/// <returns>True if the link is an internal link; otherwise, false.</returns>
		public bool IsInternalLink(string baseUrl, string link)
		{
			var baseUri = new Uri(baseUrl);
			var linkUri = new Uri(link);
			return baseUri.Host == linkUri.Host;
		}
	}
}
