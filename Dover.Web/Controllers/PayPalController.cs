using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Net;
using System.Text;
using System.IO;
using Com.Dover.Modules;
using Com.Dover.Infrastructure;

namespace Com.Dover.Controllers {
	public class PayPalController : DoverController {
		public PayPalController()
			: this(new ModuleRepository(), new AccountMembershipService()) {
		}

		public PayPalController(IModuleRepository _repo, IMembershipService _membership)
			: base(_repo, _membership) {
		}

		public ActionResult Index() {
			return View();
		}

		public ActionResult IPN() {

			var formVals = new Dictionary<string, string>();
			formVals.Add("cmd", "_notify-validate");

			string response = GetPayPalResponse(formVals);

			if (response == "VERIFIED") {

				string transactionID = Request["txn_id"];
				string sAmountPaid = Request["mc_gross"];
				string orderID = Request["custom"];

				//_logger.Info("IPN Verified for order " + orderID);

				//validate the order
				Decimal amountPaid = 0;
				Decimal.TryParse(sAmountPaid, out amountPaid);

				//Order order = _orderService.GetOrder(new Guid(orderID));
				Payment payment = null;

				//check the amount paid
				if (AmountPaidIsValid(payment, amountPaid)) {

					Address add = new Address();
					add.FirstName = Request["first_name"];
					add.LastName = Request["last_name"];
					add.Email = Request["payer_email"];
					add.Street1 = Request["address_street"];
					add.City = Request["address_city"];
					add.StateOrProvince = Request["address_state"];
					add.Country = Request["address_country"];
					add.Zip = Request["address_zip"];
					//add.UserName = order.UserName;


					//process it
					try {
						//_pipeline.AcceptPalPayment(order, transactionID, amountPaid);
						//_logger.Info("IPN Order successfully transacted: " + orderID);
						//return RedirectToAction("Receipt", "Order", new { id = order.ID });
					}
					catch {
						//HandleProcessingError(order, x);
						return View();
					}
				}
				else {
					//let fail - this is the IPN so there is no viewer
				}
			}

			return View();
		}

		/// <summary>
		/// Handles the PDT Response from PayPal
		/// </summary>
		/// <returns></returns>
		public ActionResult PDT() {
			//_logger.Info("PDT Invoked");
			string transactionID = Request.QueryString["tx"];
			string sAmountPaid = Request.QueryString["amt"];
			string orderID = Request.QueryString["cm"];
			const string PayPalPDTToken = "t5phtIH83cAliPrRRoxyUYo4Eux_Qx14ZE_vg5P-Y27dVFMbSYJa7wcoAaO";	// sandbox PDT token

			Dictionary<string, string> formVals = new Dictionary<string, string>();
			formVals.Add("cmd", "_notify-synch");
			formVals.Add("at", PayPalPDTToken);
			formVals.Add("tx", transactionID);

			string response = GetPayPalResponse(formVals);
			//_logger.Info("PDT Response received: " + response);
			if (response.StartsWith("SUCCESS")) {
				//_logger.Info("PDT Response received for order " + orderID);

				//validate the order
				Decimal amountPaid = 0;
				Decimal.TryParse(sAmountPaid, out amountPaid);

				Payment payment = null;

				if (AmountPaidIsValid(payment, amountPaid)) {

					Address add = new Address();
					add.FirstName = GetPDTValue(response, "first_name");
					add.LastName = GetPDTValue(response, "last_name");
					add.Email = GetPDTValue(response, "payer_email");
					add.Street1 = GetPDTValue(response, "address_street");
					add.City = GetPDTValue(response, "address_city");
					add.StateOrProvince = GetPDTValue(response, "address_state");
					add.Country = GetPDTValue(response, "address_country");
					add.Zip = GetPDTValue(response, "address_zip");
					//add.UserName = order.UserName;

					//process it
					try {
						// _pipeline.AcceptPalPayment(order, transactionID, amountPaid);
						// _logger.Info("PDT Order successfully transacted: " + orderID);
						//return RedirectToAction("Receipt", "Order", new { id = order.ID });
					}
					catch {
						//HandleProcessingError(order, x);
						return Content(null);
					}
				}
				else {
					//Payment amount is off
					//this can happen if you have a Gift cert at PayPal
					//be careful of this!
					//HandleProcessingError(order, new InvalidOperationException("Amount paid (" + amountPaid.ToString("C") + ") was below the order total"));
					return Content(null);
				}
			}
			else {
				ViewData["message"] = "Your payment was not successful with PayPal";
				return Content(null);
			}
			return Content(null);
		}

		/// <summary>
		/// Utility method for handling PayPal Responses
		/// </summary>
		private string GetPayPalResponse(Dictionary<string, string> formVals, bool useSandbox = true) {

			string paypalUrl = useSandbox 
				? "https://www.sandbox.paypal.com/cgi-bin/webscr"
				: "https://www.paypal.com/cgi-bin/webscr";

			HttpWebRequest req = (HttpWebRequest)WebRequest.Create(paypalUrl);

			// Set values for the request back
			req.Method = "POST";
			req.ContentType = "application/x-www-form-urlencoded";

			byte[] param = Request.BinaryRead(Request.ContentLength);
			string strRequest = Encoding.ASCII.GetString(param);

			StringBuilder sb = new StringBuilder();
			sb.Append(strRequest);

			foreach (string key in formVals.Keys) {
				sb.AppendFormat("&{0}={1}", key, formVals[key]);
			}

			strRequest += sb.ToString();
			req.ContentLength = strRequest.Length;

			//for proxy
			//WebProxy proxy = new WebProxy(new Uri("http://urlort#");
			//req.Proxy = proxy;
			//Send the request to PayPal and get the response
			string response = String.Empty;

			using (StreamWriter streamOut = new StreamWriter(req.GetRequestStream(), System.Text.Encoding.ASCII)) {
				streamOut.Write(strRequest);
				streamOut.Close();
				using (StreamReader streamIn = new StreamReader(req.GetResponse().GetResponseStream())) {
					response = streamIn.ReadToEnd();
				}
			}

			return response;
		}

		private bool AmountPaidIsValid(Payment _pmnt, decimal amountPaid) {

			//pull the order
			bool result = true;

			if (_pmnt != null) {
				if (_pmnt.Amount > amountPaid) {
					//_logger.Warn("Invalid order amount to PDT/IPN: " + order.ID + "; Actual: " + amountPaid.ToString("C") + "; Should be: " + order.Total.ToString("C") + "user IP is " + Request.UserHostAddress);
					result = false;
				}
			}
			else {
				//_logger.Warn("Invalid order ID passed to PDT/IPN; user IP is " + Request.UserHostAddress);
			}
			return result;

		}

		private string GetPDTValue(string pdt, string key) {
			string[] keys = pdt.Split('\n');
			string thisVal = String.Empty;
			string thisKey = String.Empty;

			foreach (string s in keys) {
				string[] bits = s.Split('=');
				if (bits.Length > 1) {
					thisVal = bits[1];
					thisKey = bits[0];
					if (thisKey.Equals(key, StringComparison.InvariantCultureIgnoreCase))
						break;
				}
			}
			return thisVal;
		}
	}
}