using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Elmah;
using System.Net.Mail;
using System.Net;
using System.Web.Security;
using System.Text;

namespace Com.Dover.Attributes {
	//From http://stackoverflow.com/questions/766610/
	[ValidateInput(false)]
	public class HandleErrorWithELMAHAttribute : HandleErrorAttribute {
		public override void OnException(ExceptionContext context) {
			base.OnException(context);

			var e = context.Exception;
			if (!context.ExceptionHandled		// if unhandled, will be logged anyhow
				|| RaiseErrorSignal(e)      // prefer signaling, if possible
				|| IsFiltered(context))     // filtered?
				return;

			LogException(e);
		}

		private static bool RaiseErrorSignal(Exception e) {
			var context = HttpContext.Current;
			if (context == null)
				return false;
			var signal = ErrorSignal.FromContext(context);
			if (signal == null)
				return false;
			signal.Raise(e, context);
			return true;
		}

		private static bool IsFiltered(ExceptionContext context) {
			var config = context.HttpContext.GetSection("elmah/errorFilter") as ErrorFilterConfiguration;

			if (config == null)
				return false;

			var testContext = new ErrorFilterModule.AssertionHelperContext(context.Exception, HttpContext.Current);

			return config.Assertion.Test(testContext);
		}

		private static void LogException(Exception e) {
			var context = HttpContext.Current;
			ErrorLog.GetDefault(context).Log(new Error(e, context));
			//NotifyViaEmail(e);
		}

		private static void NotifyViaEmail(Exception e) {
			try {
				var body = new StringBuilder();
				var loggedUser = Membership.GetUser();

				if (loggedUser != null) {
					body.AppendLine("User logged in: " + loggedUser.UserName);
				}

				if (e != null) {
					body.AppendLine("Exception message: " + e.Message);
					body.AppendLine("Stack trace: " + e.StackTrace);
				}

				MailMessage msg = new MailMessage(
					"dover@dovercms.com",
					"felipe@quavio.com.br",
					"Exception raised in dovercms.com",
					body.ToString());

				SmtpClient smtp = new SmtpClient();

				smtp.Credentials = new NetworkCredential("dover@dovercms.com", "uac@dmin");

				smtp.Send(msg);
			}
			catch (Exception) {
			}
		}
	}
}