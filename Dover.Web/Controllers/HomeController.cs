using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Profile;
using System.Web.Security;
using Com.Dover.Profile;
using Com.Dover.Modules;
using Br.Com.Quavio.Tools.Web.Mvc;
using Br.Com.Quavio.Tools.Web;
using System.Text;
using System.IO;
using System.Drawing;
using System.Globalization;
using Com.Dover.Helpers;
using System.Net.Mail;
using System.Net;
using Com.Dover.Attributes;

namespace Com.Dover.Controllers {
    [HandleErrorWithELMAH]
    public class HomeController : DoverController {

		public HomeController() {
		}

		public HomeController(IModuleRepository _repo, IMembershipService _membership)
			: base(_repo, _membership) {
		}
		
		[Authorize]
		public ActionResult MediaManager() {
            return PartialView();
        }

		[Authorize]
		public ActionResult Help() {
			return View();
		}

		[Authorize]
		public ActionResult MediaGallery() {
            try {
				var user = Membership.GetUser() as UACUser;

				if(user == null) {
					Response.StatusCode = 500;
					return Json(
						new { exception = "The user must be logged in to perform this action." },
						"text/html",
						JsonRequestBehavior.AllowGet);
				}
				var lstImages = user.GetImages();

                return Json(
                    lstImages.Select(imgInfo => new {
                        uploadDate = imgInfo.CreationDate.ToString("d", new CultureInfo("pt-BR")),
						imagePath = Url.Content(imgInfo.FullRelativePath),
						fileName = imgInfo.Filename,
						width = imgInfo.Width,
						height = imgInfo.Height,
						linkUrl = Request.Url.Scheme + "://" + Request.Url.Host + Url.Content(imgInfo.FullRelativePath)
                    }), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e) {
                Response.StatusCode = 500;
                return Json(
                    new { exception = e.Message },
					"text/html", 
					JsonRequestBehavior.AllowGet);
            }
        }


		[HttpPost, FlashCompatibleAuthorize]
		public ActionResult UploadImage() {
			if (Request.Files.Count == 0) {
				throw new ArgumentException();
			}

			try {
				var user = Membership.GetUser() as UACUser;

				if (user == null) {
					Response.StatusCode = 500;
					return Json(
						new { exception = "The user must be logged in to perform this action." },
						"text/html");
				}

				var imgInfo = user.SaveImage(Request.Files[0]);

				return Json(
					new {
						imagePath = Url.Content(imgInfo.FullRelativePath),
						fileName = imgInfo.Filename,
						width = imgInfo.Width,
						height = imgInfo.Height,
						linkUrl = Request.Url.Scheme + "://" + Request.Url.Host + Url.Content(imgInfo.FullRelativePath)
					},
					"text/html");
			}
			catch (Exception e) {
				Response.StatusCode = 500;
				return Json(
					new { exception = e.Message },
					"text/html");
			}
		}

		[Authorize]
		public ActionResult Error() {
            return View();
        }

		[Authorize]
		public ActionResult NotFound() {
			return View();
		}

		[Authorize]
		public ActionResult Index() {
			return View();
        }

		[Authorize]
		public ActionResult GetAnalyticsData() {
			// retrieve account analytic data
			string acctName = RouteData.Values["account"] as string;
			var analytics = new List<object>();
			var period = String.Empty;
			var totalVisits = 0;
			var culture = new CultureInfo("pt-BR");
			var gotTotals = false;

			if (acctName != null) {

				foreach (var module in ModRepository.GetAccountModules(acctName)) {
					if (!module.UsageCounters.IsLoaded) {
						module.UsageCounters.Load();
					}

					// get all module counters through last 30 days timespan
					var validCounters = module
						.UsageCounters
						.Where(uc => IsValidCounter(uc))
						.Select(uc => new {
							Date = new DateTime(uc.Year, uc.Month, uc.Day),
							Count = uc.RequestCount
					});

					if (!gotTotals && validCounters.Count() > 0) {
						period = String.Format("{0} a {1}.",
							validCounters.Select(vc => vc.Date).Min().ToString("dd/MM/yyyy", culture),
							validCounters.Select(vc => vc.Date).Max().ToString("dd/MM/yyyy", culture));

						totalVisits += validCounters.Sum(uc => uc.Count);
					}

					analytics.Add(new {
						label = module.DisplayName,
						data = validCounters.Select(uc => new long[] {
							uc.Date.ToJavaScriptTimestamp(),
							uc.Count
						})
					});
				}
			}
			else {
				var accounts = ModRepository.GetUserAccounts().OrderBy(a => a.Name);

				foreach (var account in accounts) {
					
					if (!account.Modules.IsLoaded) {
						account.Modules.Load();
					}

					var groups = ModRepository.
						GetCountersByAccountId(account.Id)
						.Where(uc => IsValidCounter(uc))
						.Select(uc => new { 
							Date = new DateTime(uc.Year, uc.Month, uc.Day),
							Count = uc.RequestCount
						}).GroupBy(uc => uc.Date);

					var validCounters = groups.Select(g => new {
						Date = g.Key,
						Count = g.Sum(gr => gr.Count)
					});

					if (!gotTotals && validCounters.Count() > 0) {
						period = String.Format("{0} a {1}",
							validCounters.Select(vc => vc.Date).Min().ToString("dd/MM/yyyy", culture),
							validCounters.Select(vc => vc.Date).Max().ToString("dd/MM/yyyy", culture));

						totalVisits += validCounters.Sum(uc => uc.Count);
					}

					analytics.Add(new {
						label = account.Name,
						data = validCounters.Select(uc => new long[] {
							uc.Date.ToJavaScriptTimestamp(),
							uc.Count
						})
					});
				}
			}

			var data = new {
				period = period,
				totalVisits = totalVisits.ToString("0,0", culture),
				plotArray = analytics
			};

			return Json(data, JsonRequestBehavior.AllowGet);
		}

		private static bool IsValidCounter(UsageCounter uc) {
			var dtNow = DateTime.Now;
			var counterDate = new DateTime(uc.Year, uc.Month, uc.Day);
			
			return counterDate.AddDays(30) >= dtNow;
		}

        [Authorize, HttpPost]
        public ActionResult CropImage(FormCollection _vars) {
			string x = _vars["xaxis"];
            string y = _vars["yaxis"];
            string w = _vars["width"];
            string h = _vars["height"];
            string relativeFilename = _vars["filename"];
			try {
				string fullOriginalPath = Server.MapPath(relativeFilename.Substring(0, relativeFilename.IndexOf("?ts")));

				int nx, ny, nw, nh;

				using (System.Drawing.Image img = System.Drawing.Image.FromFile(fullOriginalPath)) {
					nw = ParseInt(w, img.Width);
					nh = ParseInt(h, img.Height);
					nx = ParseInt(x, 0);
					ny = ParseInt(y, 0);
				}

				using (System.Drawing.Image croppedImage = System.Drawing.Image.FromStream(WebTools.Crop(fullOriginalPath, nw, nh, nx, ny), true)) {
					string saveFileName = Path.GetFileName(fullOriginalPath);

					croppedImage.Save(fullOriginalPath, croppedImage.RawFormat);

					return Json(
						new { },
						"text/html");
				}
			}
			catch (Exception e) {
				Response.StatusCode = 500;
				return Json(
					new { exception = e.Message },
					"text/html");
			}
        }

        private static int ParseInt(string s, int defaultValue) {
            int outR;
            if(Int32.TryParse(s, out outR)) {
                return outR;
            }

            return defaultValue;
        }

		public ActionResult Let_me_know() {
			return View();
		}

		public ActionResult New_home() {
			return View();
		}

		[HttpPost]
		public ActionResult Let_me_know(string email) {
			if (String.IsNullOrWhiteSpace(email)) {
				ViewModel.Success = false;
				return View(ViewModel);
			}

			try {
				var msg = new MailMessage("dovercms@dovercms.com", "felipe@quavio.com.br", "Mailing Dover", email);
				var smtp = new SmtpClient();
				smtp.Credentials = new NetworkCredential("dover@dovercms.com", "uac@dmin");
				smtp.Send(msg);
			}
			catch (Exception e) {
				ViewModel.Success = false;
				return View(ViewModel);
			}

			ViewModel.Success = true;
			return View(ViewModel);
		}
    }
}
