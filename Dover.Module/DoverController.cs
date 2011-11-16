using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Br.Com.Quavio.Tools.Web.Mvc;

namespace Com.Dover.Modules {
	public class DoverController : QuavioController {
		
		public IModuleRepository ModRepository { get; private set; }
		public IMembershipService MembershipService { get; private set; }

		public DoverController() 
			: this(new ModuleRepository(), new AccountMembershipService()) {
		}

		public DoverController(IModuleRepository _repo, IMembershipService _membership) {
			ModRepository = _repo;
			MembershipService = _membership;
		}
		
		protected override ViewResult View(string viewName, string masterName, object model) {
			if (TempData["Message"] != null) {
				ViewData["Message"] = TempData["Message"];
			}

			try {
				var repo = new ModuleRepository();
				var currAccountName = RouteData.Values["account"] as string;

				if (!String.IsNullOrWhiteSpace(currAccountName)) {
					RouteData.Values.Add("accountFriendlyName", repo.GetAccountByName(currAccountName).Name);
				}
			}
			catch {
				// TODO: Do something about this
			}

			return base.View(viewName, masterName, model);
		}
	}
}
