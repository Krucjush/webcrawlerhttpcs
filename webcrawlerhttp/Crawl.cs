using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace webcrawlerhttp
{
	public class Crawl
	{
		public List<string> getURLsFromHTML(string htmlBody, string baseURL)
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

		public string normalizeURL(string urlString)
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
