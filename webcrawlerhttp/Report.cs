using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace webcrawlerhttp
{
	public class Report
	{
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
			var saveFileDialog = new System.Windows.Forms.SaveFileDialog
			{
				Filter = "CSV File|*.csv",
				Title = "Save Report as CSV",
				FileName = "report.csv" // Default file name
			};

			if (saveFileDialog.ShowDialog() == DialogResult.OK)
			{
				var filePath = saveFileDialog.FileName;

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
