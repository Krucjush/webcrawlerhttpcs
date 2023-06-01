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
	/// Represents the main window of the application.
	/// </summary>
	public partial class MainWindow : Window
	{
		/// <summary>
		/// Gets or sets the text input for crawling.
		/// </summary>
		public string Text { get; set; }
		private readonly Crawl _crawl = new Crawl();
		private readonly Report _report = new Report();
		/// <summary>
		/// Initializes a new instance of the <see cref="MainWindow"/> class.
		/// </summary>
		public MainWindow()
		{
			InitializeComponent();
			DataContext = this;
			
		}

		/// <summary>
		/// Event handler for the button click event.
		/// </summary>
		private async void Button_Click(object sender, RoutedEventArgs e)
		{
			var progressDialog = new Progress
			{
				Owner = this
			};
			if (string.IsNullOrEmpty(Text))
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
				var pages = await Task.Run(() => _crawl.CrawlPage(Text, Text, new Crawl { Pages = new Dictionary<string, int>() }));
				progressDialog.Close();
				var sortedPages = _report.PrintReport(pages);
				MessageBox.Show(sortedPages);
			}
		}

		/// <summary>
		/// Checks if the provided URL is valid.
		/// </summary>
		/// <param name="urlString">The URL string to validate.</param>
		/// <returns><c>true</c> if the URL is valid, otherwise <c>false</c>.</returns>
		public bool IsUrlValid(string urlString)
		{
			const string pattern = @"^(http|https)://[a-zA-Z]+.[a-zA-Z]+$";

			var regex = new Regex(pattern, RegexOptions.IgnoreCase);

			var match = regex.Match(urlString);

			return match.Success;
		}

		/// <summary>
		/// Shows the progress window asynchronously.
		/// </summary>
		/// <param name="progress">The progress window to show.</param>
		public static async void ShowProgress(Window progress)
		{
			await Task.Delay(100);
			progress.Show();
		}

		/// <summary>
		/// Event handler for the second button click event.
		/// </summary>
		private void Button_Click_1(object sender, RoutedEventArgs e)
		{
			var saveFormatDialog = new SaveFormatDialog(Text, _crawl, _report);
			saveFormatDialog.ShowDialog();
		}
	}
}
