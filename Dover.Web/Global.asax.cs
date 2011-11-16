using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Data.Linq;
using Com.Dover.Modules;
using Com.Dover.Web.Models;
using Com.Dover.Web.Models.Binders;
using Com.Dover.Web.Models.DataTypes;
using Br.Com.Quavio.Tools.Web.Mvc.ModelBinders;
using System.Web.Security;
using System.Security.Principal;
using Com.Dover.Profile;
using Br.Com.Quavio.Tools.Web;
using System.IO;
using Com.Dover.Helpers.Routing;

namespace Com.Dover {
	// Note: For instructions on enabling IIS6 or IIS7 classic mode, 
	// visit http://go.microsoft.com/?LinkId=9394801

	public class DoverApplication : System.Web.HttpApplication {
		public const string Dover_Build_Major_Version = "1.1";
		public const string Dover_Build_Minor_Version = "20101027";
		public static readonly string DomainName;
		public static readonly string Scheme;

		static DoverApplication() {
#if DEBUG
			DomainName = "localdover.com";
			Scheme = "http://";
#else
			DomainName = "dovercms.com";
			Scheme = "https://";
#endif
		}
		
		public static void RegisterRoutes(RouteCollection routes) {
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
			routes.IgnoreRoute("Uploads/{*pathInfo}");
			routes.IgnoreRoute("Content/{*pathInfo}");
			routes.IgnoreRoute("Scripts/{*pathInfo}");
			routes.IgnoreRoute("ckfinder/userfiles/images/{*pathInfo}");
			routes.IgnoreRoute("ImageHandler.ashx");
			routes.IgnoreRoute("robots.txt");

			AreaRegistration.RegisterAllAreas();

			routes.Add("ApiRouteFormat", new DomainRoute(
				"api." + DomainName,
				"{account}/{action}/{moduleid}/{id}.{format}",
				new { controller = "Api", action = "module", account = "",  moduleid = "", format = "xml", id = UrlParameter.Optional }
			));

			routes.Add("ApiRoute", new DomainRoute(
				"api." + DomainName,
				"{account}/{action}/{moduleid}.{format}",
				new { controller = "Api", action = "module", account = "", moduleid = "", format = "xml" }
			));

			routes.Add("HelpRoute", new DomainRoute(
				"help." + DomainName,
				"{action}",
				new { controller = "Help", action = "Index" }
			));

			routes.MapRoute("Let_me_know", "let_me_know", new { controller = "Home", action = "Let_me_know" });
			routes.MapRoute("New_home", "New_home", new { controller = "Home", action = "New_home" });
			routes.MapRoute("OpenId", "OpenId", new { controller = "Account", action = "OpenId" });
			routes.MapRoute("Signup", "Signup", new { controller = "Account", action = "Signup" });

			routes.MapRoute(
				"Dynamic_modules_query",
				"dyn/{moduleid}/{modulename}/Query{format}/{id}",
				new { controller = "DynamicModule", modulename = "", moduleid = "", action = "Query", format = ".xml", id = UrlParameter.Optional }
			);

			routes.Add(
				"Dynamic_modules_route", new DomainRoute(
				"{account}." + DomainName,
				"dyn/{moduleid}/{modulename}/{action}/{id}",
				new { moduleid = "", modulename = "", controller = "DynamicModule", action = "List", id = UrlParameter.Optional }
			));

			routes.Add(
				"Default", new DomainRoute(
				"{account}." + DomainName,
				"{controller}/{action}/{id}",
				new { controller = "Home", action = "Index", account = UrlParameter.Optional, id = UrlParameter.Optional }
			));

			//RouteDebug.RouteDebugger.RewriteRoutesForTesting(RouteTable.Routes);
		}

		protected void Application_Start() {
			RegisterRoutes(RouteTable.Routes);

			ModelBinders.Binders.Add(
				typeof(DynamicModuleViewModel),
				new DynamicModuleModelBinder());

			ModelBinders.Binders.Add(
				typeof(DbImage),
				new DbImageModelBinder());

			ModelBinders.Binders.Add(
				typeof(ImageList),
				new ImageListModelBinder());

			ModelBinders.Binders.Add(
				typeof(Nullable<DateTime>),
				new DateTimeModelBinder());

			ModelBinders.Binders.Add(
				typeof(DateTime),
				new DateTimeModelBinder());

			ModelBinders.Binders.Add(
				typeof(Com.Dover.Web.Models.DataTypes.File),
				new FileModelBinder());
		}

		protected void Application_BeginRequest(Object sender, EventArgs e) {
			// This Response filter messes with WebResource.axd for some reason and the content is blank.
			// We'll skip it or DotNetOpenAuth won't work.
			/*if (!HttpContext.Current.Request.Url.AbsolutePath.Contains("WebResource.axd")) {
				this.Response.Filter = new ObserveResponseLengthStream(this.Response.Filter);
			}*/
			
			var req = HttpContext.Current.Request;

			if (req.Url.Host.Contains("dovercms.com.br")) {
				Response.Redirect("https://" + req.Url.Host.TrimEnd(".br".ToCharArray()) + req.RawUrl);
			}
			else if (!req.IsSecureConnection &&
				req.Url.Host.Contains("dovercms.com")) {
				Response.Redirect("https://" + Request.ServerVariables["HTTP_HOST"] + req.RawUrl);
			}
			else {
				var subdomain = req.Url.GetSubDomain();

				// always trim "www" away from the url
				if (subdomain != null && subdomain == "www") {
					Response.Redirect("http://" + req.ServerVariables["HTTP_HOST"].TrimStart("w.".ToCharArray()) + req.RawUrl);
				}
			}
		}
		protected void Application_EndRequest(Object sender, EventArgs e) {
			/*if (!HttpContext.Current.Request.Url.AbsolutePath.Contains("WebResource.axd")) {
				var request = Request.Path;
				var responseLength = this.Response.Filter.Length;
			}*/
		}
	}

	class ObserveResponseLengthStream : Stream {
		private Stream _stream = null;
		private long _length = 0;
		public ObserveResponseLengthStream(Stream stream) {
			this._stream = stream;
		}

		public override long Seek(long offset, SeekOrigin origin) {
			return this._stream.Seek(offset, origin);
		}

		public override void Flush() {
			this._stream.Flush();
		}

		public override void SetLength(long value) {
			this._stream.SetLength(value);
			this._length = value;
		}

		public override void Write(byte[] buffer, int offset, int count) {
			this._stream.Write(buffer, offset, count);
			this._length += (long)count;
		}

		public override void Close() {
			try {
				this._stream.Close();
			}
			catch { }

			base.Close();
		}

		public override bool CanRead {
			get { return _stream.CanRead; }
		}

		public override bool CanSeek {
			get { return _stream.CanSeek; }
		}

		public override bool CanWrite {
			get { return _stream.CanWrite; }
		}

		public override long Length {
			get { return _length; }
		}

		public override long Position {
			get { return _stream.Position; }
			set { _stream.Position = value; }
		}

		public override int Read(byte[] buffer, int offset, int count) {
			return _stream.Read(buffer, offset, count);
		}
	}
}