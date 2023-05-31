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
	/// Interaction logic for SaveFormatDialog.xaml
	/// </summary>
	public partial class SaveFormatDialog : Window
	{
		private string _text { get; set; }
		private Crawl _crawl { get; set; }
		private Report _report { get; set; }
		public SaveFormatDialog(string text, Crawl crawl, Report report)
		{
			_text = text;
			_crawl = crawl;
			_report = report;
			InitializeComponent();
		}

		private async void saveButton_Click(object sender, RoutedEventArgs e)
		{
			if (xmlRadioButton.IsChecked == true)
			{
				var progressDialog = new Progress
				{
					Owner = this,
				};
				ShowProgress(progressDialog);
				var pages = await Task.Run(() => _crawl.CrawlPage(_text, _text, new Crawl { Pages = new Dictionary<string, int>() }));
				progressDialog.Close();
				var xmlContent = _report.GenerateXmlReport(pages);
				await _report.SaveReportAsXml(System.Net.WebUtility.HtmlDecode(xmlContent));
			}
			else if (csvRadioButton.IsChecked == true)
			{
				var progressDialog = new Progress
				{
					Owner = this,
				};
				ShowProgress(progressDialog);
				var pages = await Task.Run(() => _crawl.CrawlPage(_text, _text, new Crawl { Pages = new Dictionary<string, int>() }));
				progressDialog.Close();
				var csvContent = _report.GenerateCsvReport(pages);
				await _report.SaveReportAsCsv(csvContent);
			}
			else if (jsonRadioButton.IsChecked == true)
			{
				var progressDialog = new Progress
				{
					Owner = this,
				};
				ShowProgress(progressDialog);
				var pages = await Task.Run(() => _crawl.CrawlPage(_text, _text, new Crawl { Pages = new Dictionary<string, int>() }));
				progressDialog.Close();
				var jsonContent = _report.GenerateJsonReport(pages);
				await _report.SaveReportAsJson(jsonContent);
			}
			else 
			{
				MessageBox.Show("Something went wrong");
			}
			Close();
		}
		private async void ShowProgress(Progress progress)
		{
			await Task.Delay(100);
			progress.Show();
		}
	}
}