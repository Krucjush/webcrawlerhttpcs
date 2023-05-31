using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace webcrawlerhttp
{
	public interface IReportData
	{
		Dictionary<string, int> Pages { get; }
		int InternalLinksCount { get; }
		int ExternalLinksCount { get; }
	}
}
