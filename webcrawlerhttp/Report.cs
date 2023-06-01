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
	/// <summary>
	/// Represents a class that generates and saves reports.
	/// </summary>
	public class Report
	{
		private readonly System.Windows.Forms.SaveFileDialog _saveFileDialog = new System.Windows.Forms.SaveFileDialog();

		/// <summary>
		/// Generates an XML report based on the provided report data.
		/// </summary>
		/// <typeparam name="T">The type of report data.</typeparam>
		/// <param name="reportData">The report data.</param>
		/// <returns>The generated XML report as a string.</returns>
		public string GenerateXmlReport<T>(T reportData) where T : IReportData
		{
			var xmlContent = new XElement("Report");

			var sortedPages = SortPages(reportData.Pages);

			foreach (var pageElement in from sortedPage in sortedPages let url = sortedPage.Key let hits = sortedPage.Value select new XElement("Page",
				         new XElement("URL", url),
				         new XElement("Hits", hits)
			         ))
			{
				xmlContent.Add(pageElement);
			}

			xmlContent.Add(new XElement("InternalLinksCount", reportData.InternalLinksCount));
			xmlContent.Add(new XElement("ExternalLinksCount", reportData.ExternalLinksCount));

			return xmlContent.ToString();
		}

		/// <summary>
		/// Saves the report as XML to the specified file path.
		/// </summary>
		/// <param name="pages">The report pages as an XML string.</param>
		/// <returns>A task representing the asynchronous operation.</returns>
		public async Task SaveReportAsXml(string pages)
		{
			_saveFileDialog.Filter = "XML File|*.xml";
			_saveFileDialog.Title = "Save Report as XML";
			_saveFileDialog.FileName = "report.xml"; // Default file name

			if (_saveFileDialog.ShowDialog() == DialogResult.OK)
			{
				var filePath = _saveFileDialog.FileName;

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

		/// <summary>
		/// Generates a JSON report based on the provided crawl data.
		/// </summary>
		/// <typeparam name="T">The type of crawl data.</typeparam>
		/// <param name="crawl">The crawl data.</param>
		/// <returns>The generated JSON report as a dictionary.</returns>
		public Dictionary<string, int> GenerateJsonReport<T>(T crawl) where T : IReportData
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

		/// <summary>
		/// Saves the report as JSON to the specified file path.
		/// </summary>
		/// <param name="pages">The report pages as a dictionary.</param>
		/// <returns>A task representing the asynchronous operation.</returns>
		public async Task SaveReportAsJson(Dictionary<string, int> pages)
		{
			_saveFileDialog.Filter = "JSON File|*.json";
			_saveFileDialog.Title = "Save Report as JSON";
			_saveFileDialog.FileName = "report.json"; // Default file name
			var jsonSettings = new JsonSerializerSettings
			{
				StringEscapeHandling = StringEscapeHandling.EscapeHtml
			};

			if (_saveFileDialog.ShowDialog() == DialogResult.OK)
			{
				var filePath = _saveFileDialog.FileName;

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

		/// <summary>
		/// Generates a CSV report based on the provided crawl data.
		/// </summary>
		/// <typeparam name="T">The type of crawl data.</typeparam>
		/// <param name="crawl">The crawl data.</param>
		/// <returns>The generated CSV report as a string.</returns>
		public string GenerateCsvReport<T>(T crawl) where T : IReportData
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

		/// <summary>
		/// Saves the report as CSV to the specified file path.
		/// </summary>
		/// <param name="csvContent">The report content as a CSV string.</param>
		/// <returns>A task representing the asynchronous operation.</returns>
		public async Task SaveReportAsCsv(string csvContent)
		{
			_saveFileDialog.Filter = "CSV File|*.csv";
			_saveFileDialog.Title = "Save Report as CSV";
			_saveFileDialog.FileName = "report.csv"; // Default file name

			if (_saveFileDialog.ShowDialog() == DialogResult.OK)
			{
				var filePath = _saveFileDialog.FileName;

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

		/// <summary>
		/// Prints the report based on the provided crawl data.
		/// </summary>
		/// <typeparam name="T">The type of crawl data.</typeparam>
		/// <param name="crawl">The crawl data.</param>
		/// <returns>The printed report as a string.</returns>
		public string PrintReport<T>(T crawl) where T : IReportData
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

		/// <summary>
		/// Sorts the pages in the report based on the number of hits.
		/// </summary>
		/// <param name="pages">The dictionary of pages to sort.</param>
		/// <returns>The sorted dictionary of pages.</returns>
		public Dictionary<string, int> SortPages(Dictionary<string, int> pages)
		{
			var sortedURLs = pages.OrderByDescending(q => q.Value)
				.ToDictionary(q => q.Key, p => p.Value);
			return sortedURLs;
		}
	}
}
