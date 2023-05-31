using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace webcrawlerhttp
{
	public class Report
	{
		private System.Windows.Forms.SaveFileDialog SaveFileDialog = new System.Windows.Forms.SaveFileDialog();

		public string GenerateXmlReport(Crawl crawl)
		{
			var xmlContent = new XElement("Report");

			var sortedPages = SortPages(crawl.Pages);

			foreach (var sortedPage in sortedPages)
			{
				var url = sortedPage.Key;
				var hits = sortedPage.Value;

				var pageElement = new XElement("Page",
					new XElement("URL", url),
					new XElement("Hits", hits)
				);

				xmlContent.Add(pageElement);
			}

			xmlContent.Add(new XElement("InternalLinksCount", crawl.InternalLinksCount));
			xmlContent.Add(new XElement("ExternalLinksCount", crawl.ExternalLinksCount));

			return xmlContent.ToString();
		}

		public async Task SaveReportAsXml(string pages)
		{
			SaveFileDialog.Filter = "XML File|*.xml";
			SaveFileDialog.Title = "Save Report as XML";
			SaveFileDialog.FileName = "report.xml"; // Default file name

			if (SaveFileDialog.ShowDialog() == DialogResult.OK)
			{
				var filePath = SaveFileDialog.FileName;

				try
				{
					using (var writer = new StreamWriter(filePath))
					{
						await writer.WriteAsync(pages);
					}
					MessageBox.Show("Report saved as XML successfully.");
				}
				catch (Exception ex)
				{
					MessageBox.Show($"Error saving report: {ex.Message}");
				}
			}
		}

		public Dictionary<string, int> GenerateJsonReport(Crawl crawl)
		{
			var report = new Dictionary<string, int>();

			var sortedPages = SortPages(crawl.Pages);

			foreach (var sortedPage in sortedPages)
			{
				report[sortedPage.Key] = sortedPage.Value;
			}

			report["InternalLinksCount"] = crawl.InternalLinksCount;
			report["ExternalLinksCount"] = crawl.ExternalLinksCount;

			return report;
		}

		public async Task SaveReportAsJson(Dictionary<string, int> pages)
		{
			SaveFileDialog.Filter = "JSON File|*.json";
			SaveFileDialog.Title = "Save Report as JSON";
			SaveFileDialog.FileName = "report.json"; // Default file name
			var jsonSettings = new JsonSerializerSettings
			{
				StringEscapeHandling = StringEscapeHandling.EscapeHtml
			};

			if (SaveFileDialog.ShowDialog() == DialogResult.OK)
			{
				var filePath = SaveFileDialog.FileName;

				try
				{
					var jsonContent = JsonConvert.SerializeObject(pages, Formatting.Indented, jsonSettings);
					using (var writer = new StreamWriter(filePath))
					{
						await writer.WriteAsync(jsonContent);
					}
					MessageBox.Show("Report saved as JSON successfully.");
				}
				catch (Exception ex)
				{
					MessageBox.Show($"Error saving report: {ex.Message}");
				}
			}
		}

		public string GenerateCsvReport(Crawl crawl)
		{
			var csvContent = "URL,Hits\n";

			var sortedPages = SortPages(crawl.Pages);

			foreach (var sortedPage in sortedPages)
			{
				var url = sortedPage.Key;
				var hits = sortedPage.Value;
				csvContent += $"{url},{hits}\n";
			}
			csvContent += $"internal links count,{crawl.InternalLinksCount}\n";
			csvContent += $"external links count,{crawl.ExternalLinksCount}\n";

			return csvContent;
		}

		public async Task SaveReportAsCsv(string csvContent)
		{
			SaveFileDialog.Filter = "CSV File|*.csv";
			SaveFileDialog.Title = "Save Report as CSV";
			SaveFileDialog.FileName = "report.csv"; // Default file name

			if (SaveFileDialog.ShowDialog() == DialogResult.OK)
			{
				var filePath = SaveFileDialog.FileName;

				try
				{
					using (var writer = new StreamWriter(filePath))
					{
						await writer.WriteAsync(csvContent);
					}
					MessageBox.Show("Repot saved as CSV successfully.");
				}
				catch (Exception ex)
				{
					MessageBox.Show($"Error saving report: {ex.Message}");
				}
			}
		}

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

			output += $"\nTotal external links: {crawl.ExternalLinksCount}\n";
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
