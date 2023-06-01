using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace webcrawlerhttp
{
	/// <summary>
	/// Represents the data required for generating a report.
	/// </summary>
	public interface IReportData
	{
		/// <summary>
		/// Gets the dictionary of pages and their hit counts.
		/// </summary>
		Dictionary<string, int> Pages { get; }
		/// <summary>
		/// Gets the count of internal links.
		/// </summary>
		int InternalLinksCount { get; }
		/// <summary>
		/// Gets the count of external links.
		/// </summary>
		int ExternalLinksCount { get; }
	}
}
