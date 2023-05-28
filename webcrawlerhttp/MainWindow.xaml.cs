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
			var http = "\"https://blog.boot.dev/path1/\"";
			var relative = "\"/path2/\"";
			var html = $"<html>\r\n    <body>\r\n        <a href={http}>\r\n  <a href={relative}>\r\n           Boot.dev Blog\r\n        </a>\r\n    </body>\r\n</html>";
			var output = "Output:\n";
			foreach (var item in _crawl.getURLsFromHTML(html, http))
			{
				output+= item + "\n";
			}
			MessageBox.Show(output);
		}
	}
}
