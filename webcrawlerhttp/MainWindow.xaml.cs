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
		public MainWindow()
		{
			InitializeComponent();
			DataContext = this;
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			const string noWeb = "no website provided";
			if (Text == null)
			{
				MessageBox.Show(noWeb);
			}
			else if (Text.Length < 1)
			{
				MessageBox.Show(noWeb);
			}
			else if (Text.Contains(' '))
			{
				MessageBox.Show("too many arguments provided");
			}
			else
			{
				MessageBox.Show($"starting crawl of {Text}");
				_crawl.CrawlPage(Text);
			}
		}
	}
}
