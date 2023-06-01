using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;
using webcrawlerhttp;

namespace webcrawlerhttp
{
	/// <summary>
	/// Represents a dialog window for selecting and saving report formats.
	/// </summary>
	public partial class SaveFormatDialog : Window
	{
		/// <summary>
		/// The text to be used for generating the report.
		/// </summary>
		private string _text { get; set; }
		/// <summary>
		/// The crawl object used for crawling web pages.
		/// </summary>
		private Crawl _crawl { get; set; }
		/// <summary>
		/// The report object used for generating and saving reports.
		/// </summary>
		private Report _report { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="SaveFormatDialog"/> class.
		/// </summary>
		/// <param name="text">The text to be used for generating the report.</param>
		/// <param name="crawl">The crawl object used for crawling web pages.</param>
		/// <param name="report">The report object used for generating and saving reports.</param>
		public SaveFormatDialog(string text, Crawl crawl, Report report)
		{
			_text = text;
			_crawl = crawl;
			_report = report;
			InitializeComponent();
		}

		/// <summary>
		/// Handles the click event of the saveButton.
		/// Saves the report in the selected format (XML, CSV, or JSON) based on the user's selection.
		/// </summary>
		/// <param name="sender">The sender of the event.</param>
		/// <param name="e">The event arguments.</param>
		private async void saveButton_Click(object sender, RoutedEventArgs e)
		{
			if (xmlRadioButton.IsChecked == true)
			{
				// Generate and save the report as XML.
				var progressDialog = new Progress
				{
					Owner = this,
				};
				MainWindow.ShowProgress(progressDialog);
				var pages = await Task.Run(() => _crawl.CrawlPage(_text, _text, new Crawl { Pages = new Dictionary<string, int>() }));
				progressDialog.Close();
				var xmlContent = _report.GenerateXmlReport(pages);
				await _report.SaveReportAsXml(System.Net.WebUtility.HtmlDecode(xmlContent));
			}
			else if (csvRadioButton.IsChecked == true)
			{
				// Generate and save the report as CSV.
				var progressDialog = new Progress
				{
					Owner = this,
				};
				MainWindow.ShowProgress(progressDialog);
				var pages = await Task.Run(() => _crawl.CrawlPage(_text, _text, new Crawl { Pages = new Dictionary<string, int>() }));
				progressDialog.Close();
				var csvContent = _report.GenerateCsvReport(pages);
				await _report.SaveReportAsCsv(csvContent);
			}
			else if (jsonRadioButton.IsChecked == true)
			{
				// Generate and save the report as JSON.
				var progressDialog = new Progress
				{
					Owner = this,
				};
				MainWindow.ShowProgress(progressDialog);
				var pages = await Task.Run(() => _crawl.CrawlPage(_text, _text, new Crawl { Pages = new Dictionary<string, int>() }));
				progressDialog.Close();
				var jsonContent = _report.GenerateJsonReport(pages);
				await _report.SaveReportAsJson(jsonContent);
			}
			else 
			{
				// Display an error message if no format is selected.
				MessageBox.Show("Something went wrong");
			}
			Close();
		}
	}
}