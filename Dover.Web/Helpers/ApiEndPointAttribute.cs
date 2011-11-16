using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Com.Dover.Web.Models;
using Com.Dover.Web.Helpers;

namespace Com.Dover.Helpers {
	public class ApiEndPointAttribute : ActionFilterAttribute {
		public ApiEndPointAttribute() {
			DefaultToXml = true;
		}

		public bool DefaultToXml { get; set; }

		private XmlWriterSettings xmlSettings = new XmlWriterSettings() {
			OmitXmlDeclaration = true,
			Encoding = UTF8,
			Indent = true
		};

		private static UTF8Encoding UTF8 = new UTF8Encoding(false);

		public override void OnActionExecuted(ActionExecutedContext filterContext) {
			// setup the request, view and data 
			ViewResult view = filterContext.Result as ViewResult;

			if (view == null) {
				filterContext.Result = new EmptyResult();
				return;
			}

			var data = view.ViewData.Model;

			if (data == null) {
				filterContext.Result = new EmptyResult();
				return;
			}

			// JSON 
			if (IsJson(filterContext)) {
				filterContext.Result = new JsonResult {
					Data = data,
					JsonRequestBehavior = JsonRequestBehavior.AllowGet
				};
			}
			else if (IsCsv(filterContext)) {
				filterContext.Result = new CsvResult {
					Data = data as DynamicModuleApiResult
				};
			}
			// POX 
			else if (DefaultToXml || IsPox(filterContext)) {
				// MemoryStream to encapsulate as UTF-8 (default UTF-16) 
				// http://stackoverflow.com/questions/427725/ 
				using (MemoryStream stream = new MemoryStream(500)) {
					using (var xmlWriter = XmlTextWriter.Create(stream, xmlSettings)) {
						new XmlSerializer(data.GetType(), new XmlRootAttribute(data.ToString())).Serialize(xmlWriter, data);
					}
					filterContext.Result = new ContentResult {
						ContentType = "text/xml",
						Content = UTF8.GetString(stream.ToArray()),
						ContentEncoding = UTF8
					};
				}
			}
		}

		private bool IsJson(ActionExecutedContext filterContext) {
			HttpRequestBase request;
			String contentType;
			var routeValues = filterContext.RequestContext.RouteData.Values;

			try {
				request = filterContext.RequestContext.HttpContext.Request;
				contentType = request.ContentType ?? string.Empty;
			}
			catch (NotImplementedException) {
				return false;
			}

			return contentType.Contains("application/json") ||
				request.QueryString["format"] == "json" ||
				(routeValues["format"] != null &&
				routeValues["format"].ToString().Contains("json"));

		}

		private bool IsCsv(ActionExecutedContext filterContext) {
			HttpRequestBase request;
			String contentType;
			var routeValues = filterContext.RequestContext.RouteData.Values;

			try {
				request = filterContext.RequestContext.HttpContext.Request;
				contentType = request.ContentType ?? string.Empty;
			}
			catch (NotImplementedException) {
				return false;
			}

			return contentType.Contains("text/csv") ||
				contentType.Contains("text/comma-separated-values") ||
				request.QueryString["format"] == "csv" ||
				(routeValues["format"] != null &&
				routeValues["format"].ToString().Contains("csv"));

		}

		private bool IsPox(ActionExecutedContext filterContext) {
			if (filterContext.RequestContext == null) {
				return true; // default to XML
			}

			var request = filterContext.RequestContext.HttpContext.Request;
			var contentType = request.ContentType ?? string.Empty;
			var routeValues = filterContext.RequestContext.RouteData.Values;

			return contentType.Contains("text/xml") ||
				request.QueryString["format"] == "xml" ||
				(routeValues["format"] != null &&
				routeValues["format"].ToString().Contains("xml"));
		}
	}
}