using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace webcrawlerhttp
{
	public class Report
	{
		private System.Windows.Forms.SaveFileDialog SaveFileDialog = new System.Windows.Forms.SaveFileDialog();

		public async Task SaveReportAsXml(Dictionary<string, int> pages)
		{
			SaveFileDialog.Filter = "XML File|*.xml";
			SaveFileDialog.Title = "Save Report as XML";
			SaveFileDialog.FileName = "report.xml"; // Default file name

			if (SaveFileDialog.ShowDialog() == DialogResult.OK)
			{
				var filePath = SaveFileDialog.FileName;

				try
				{
					var serializer = new XmlSerializer(pages.GetType());
					using (var writer = new StreamWriter(filePath))
					{
						await Task.Run(() => serializer.Serialize(writer, pages));
					}
					MessageBox.Show("Report saved as XML successfully.");
				}
				catch (Exception ex)
				{
					MessageBox.Show($"Error saving report: {ex.Message}");
				}
			}
		}

		public async Task SaveReportAsJson(Dictionary<string, int> pages)
		{
			SaveFileDialog.Filter = "JSON File|*.json";
			SaveFileDialog.Title = "Save Report as JSON";
			SaveFileDialog.FileName = "report.json"; // Default file name

			if (SaveFileDialog.ShowDialog() == DialogResult.OK)
			{
				var filePath = SaveFileDialog.FileName;

				try
				{
					var jsonContent = JsonConvert.SerializeObject(pages, Formatting.Indented);
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

		public string GenerateCsvReport(Dictionary<string, int> pages)
		{
			var csvContent = "URL,Hits\n";

			var sortedPages = SortPages(pages);

			foreach (var sortedPage in sortedPages)
			{
				var url = sortedPage.Key;
				var hits = sortedPage.Value;
				csvContent += $"{url},{hits}\n";
			}
			
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
