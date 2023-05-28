using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace webcrawlerhttp
{
	public class Crawl
	{
		public async void CrawlPage(string currentURL)
		{
			MessageBox.Show($"actively crawling {currentURL}");
			try
			{
				var httpClient = new HttpClient();
				var resp = await httpClient.GetAsync(currentURL);

				if ((int)resp.StatusCode > 399)
				{
					MessageBox.Show($"error in fetch with status code: {(int)resp.StatusCode} on page: {currentURL}");
				}
				else
				{
					var contentType = resp.Content.Headers.GetValues("Content-Type").FirstOrDefault();
					if (!contentType.Contains("text/html"))
					{
						MessageBox.Show($"non html response, content type: {contentType}, on page: {currentURL}");
					}
					else
					{
						MessageBox.Show(await resp.Content.ReadAsStringAsync());
					}
				}
			}
			catch (Exception err)
			{
				MessageBox.Show($"error in fetch: {err.Message}, on page {currentURL}");
			}
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
							var urlObj = new Uri(baseURL.Substring(1, baseURL.Length - 2) + href);
							urls.Add(urlObj.Host + urlObj.AbsolutePath);
						}
						catch (Exception err)
						{
							MessageBox.Show($"error with relative url:\n{err.Message}");
						}
					}
					else
					{
						//absolute
						try
						{
							var urlObj = new Uri(href);
							urls.Add(urlObj.Host + urlObj.AbsolutePath);
						}
						catch (Exception err)
						{
							MessageBox.Show($"error with absolute url:\n{err.Message}");
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
	}
}
