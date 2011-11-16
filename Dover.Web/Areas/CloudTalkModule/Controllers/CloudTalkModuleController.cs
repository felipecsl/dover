using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Com.Dover.Helpers;
using Com.Dover.Modules;
using Com.Dover.Attributes;
using Com.Dover.Web.Areas.CloudTalkModule.Models;
using Com.Dover.Areas.CloudTalkModule.Models;

namespace Com.Dover.Areas.CloudTalkModule.Controllers {
	[HandleErrorWithELMAH, Authorize]
	public class CloudTalkModuleController : ModuleController {

		//
		// GET: /CloudTalkModule/CloudTalkModule/

		public ActionResult Index() {
			return View();
		}

		public ActionResult Start() {
			try {
				var clntName = GetClientId();
				return Redirect("http://mensagei.web292.uni5.net/Client.aspx/Dashboard/" + clntName);
			}
			catch (ArgumentException e) {
				TempData["Message"] = e.Message;
				return View();
			}
		}

		public ActionResult History() {
			try {
				var clntId = GetClientId();

				using (var db = new CloudTalkEntities()) {
					var entries = db.History.Where(h => h.ClientId == clntId).ToList();

					return View(entries.Select(e => new CloudTalkHistoryViewModel {
						SenderEmail = e.Sender,
						Message = e.Text.Replace("{ Message = ", "").Replace("}", ""),
						TimeStamp = e.Timestamp
					}).ToList());
				}
			}
			catch (ArgumentException e) {
				TempData["Message"] = e.Message;
				return View();
			}
		}

		public ActionResult MenuItem() {
			return View("~/Areas/CloudTalkModule/Views/CloudTalkModule/CloudTalkMenuItem.ascx");
		}

		private int GetClientId() {
			var acctName = RouteData.Values["account"] as string;

			if (String.IsNullOrWhiteSpace(acctName)) {
				throw new ArgumentException("Conta não encontrada.");
			}

			var acct = ModRepository.GetAccountByName(acctName);

			if (acct == null) {
				throw new ArgumentException("Conta não encontrada.");
			}

			using (var db = new CloudTalkModuleEntities()) {
				var entry = db.StaticModule_CloudTalk.FirstOrDefault(c => c.AccountId == acct.Id);

				if (entry == null) {
					throw new ArgumentException("Dados de atendimento não encontrados.");
				}

				return entry.CloudTalkId;
			}
		}
	}
}