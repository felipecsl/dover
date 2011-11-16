using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Com.Dover.Helpers {
	public static class DoverHtmlHelper {
		public static string GetAccountName() {
			var req = HttpContext.Current.Request;
			string host = req.Url.Host;

			if (host.Split('.').Length <= 2) {
				return null;
			}

			int lastIndex = host.LastIndexOf(".");
			int index = host.LastIndexOf(".", lastIndex - 1);
			return host.Substring(0, index);
		}
		
		public static string GetAccountUrl(this HtmlHelper helper, string accountName) {
			var req = helper.ViewContext.HttpContext.Request;
			string host = req.Url.Host;

			if (host.Split('.').Length > 2) {
				int lastIndex = host.LastIndexOf(".");
				int index = host.LastIndexOf(".", lastIndex - 1);
				string subdomain = host.Substring(0, index);
				return req.Url.Scheme + "://" + host.Replace(subdomain, accountName);
			}
			else {
				return String.Format("{0}://{1}.{2}", req.Url.Scheme, accountName, host);
			}
		}

		public static string GetModuleApiUrl(this HtmlHelper helper, string accountName, string moduleName, string moduleId, string id, string format = "xml") {
			var req = helper.ViewContext.HttpContext.Request;
			string host = req.Url.Host;

			if (id != null) {
				return String.Format("{0}://api.{1}/{2}/module/{3}/{4}.{5}",
					req.Url.Scheme,
					DoverApplication.DomainName,
					accountName,
					moduleId,
					id,
					format);
			}
			else {
				return String.Format("{0}://api.{1}/{2}/module/{3}.{4}",
					req.Url.Scheme,
					DoverApplication.DomainName,
					accountName,
					moduleId,
					format);
			}
		}

		public static string GetDashboardUrl(this HtmlHelper helper) {
			var req = helper.ViewContext.HttpContext.Request;
			string host = req.Url.Host;

			if (host.Split('.').Length > 2) {
				int lastIndex = host.LastIndexOf(".");
				int index = host.LastIndexOf(".", lastIndex - 1);
				string subdomain = host.Substring(0, index);
				return req.Url.Scheme + "://" + host.Replace(subdomain + ".", String.Empty);
			}
			else {
				return req.Url.Scheme + "://" + req.Url.Host;
			}
		}
	}
}