using System.Web.Mvc;
using Com.Dover.Helpers.Routing;

namespace Com.Dover.Areas.CloudTalkModule {
	public class CloudTalkModuleAreaRegistration : AreaRegistration {
		public override string AreaName {
			get {
				return "CloudTalkModule";
			}
		}

		public override void RegisterArea(AreaRegistrationContext context) {
			var route = new DomainRoute(
				"{account}." + DoverApplication.DomainName,
				"CloudTalkModule/{action}/{id}",
				new { controller = "CloudTalkModule", action = "Index", id = UrlParameter.Optional }
			);

			route.Area = AreaName;

			context.Routes.Add("CloudTalkModuleRoute", route);

			//context.MapRoute(
			//    "CloudTalkModuleRoute",
			//    "CloudTalkModule/{action}/{id}",
			//    new { controller = "CloudTalkModule", action = "Index", id = UrlParameter.Optional }
			//);
		}
	}
}
