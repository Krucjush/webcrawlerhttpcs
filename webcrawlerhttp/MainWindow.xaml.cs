using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace webcrawlerhttp
{
	/// <summary>
	/// Logika interakcji dla klasy MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public string Text { get; set; }
		private Crawl _crawl = new Crawl();
		private Report _report = new Report();
		public MainWindow()
		{
			InitializeComponent();
			DataContext = this;
			
		}

		private async void Button_Click(object sender, RoutedEventArgs e)
		{
			var progressDialog = new Progress
			{
				Owner = this
			};
			if (Text == null || Text.Length < 1)
			{
				MessageBox.Show("no website provided");
			}
			else if (!IsUrlValid(Text))
			{
				MessageBox.Show("wrong input");
			}
			else
			{
				MessageBox.Show($"starting crawl of {Text}");
				ShowProgress(progressDialog);
				var pages = await Task.Run(() => _crawl.CrawlPage(Text, Text, new Dictionary<string, int>()));
				progressDialog.Close();
				var sortedPages = _report.PrintReport(pages);
				MessageBox.Show(sortedPages);
			}
		}
		public bool IsUrlValid(string urlString)
		{
			var pattern = @"^(http|https)://";

			var regex = new Regex(pattern, RegexOptions.IgnoreCase);

			var match = regex.Match(urlString);

			return match.Success;
		}
		private async void ShowProgress(Progress progress)
		{
			await Task.Delay(100);
			progress.Show();
		}
	}
}
