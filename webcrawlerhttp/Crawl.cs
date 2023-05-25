using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace webcrawlerhttp
{
	public class Crawl
	{
		public string normalizeURL(string urlString)
		{
			var urlObj = new Uri(urlString);
			var hostPath = $"{urlObj.Host}{urlObj.AbsolutePath}";
			if (hostPath.Length > 0 && hostPath.Substring(hostPath.Length - 1, 1)[0] == '/')
			{
				return hostPath.Substring(0, hostPath.Length-1);
			}
			return hostPath;
		}
	}
}
