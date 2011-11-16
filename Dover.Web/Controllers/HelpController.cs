using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Com.Dover.Web.Controllers {
	public class HelpController : Controller {
		//
		// GET: /Help/

		public ActionResult Index() {
			return View();
		}

		public ActionResult Getting_Started() {
			return View();
		}
	}
}