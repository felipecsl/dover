using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI;
using System.Web.Profile;
using Com.Dover.Web.Models;
using Com.Dover.Profile;
using Com.Dover.Modules;
using Br.Com.Quavio.Tools.Web;
using System.Net.Mail;
using System.Net;
using System.Diagnostics.CodeAnalysis;
using System.Web.Script.Serialization;
using System.Text;
using Com.Dover.Helpers;
using System.Configuration.Provider;
using Com.Dover.Attributes;
using Com.Dover.Infrastructure;

namespace Com.Dover.Controllers {
	[HandleErrorWithELMAH]
	public class AccountController : DoverController {
		public AccountController()
			: this(
			new ModuleRepository(),
			new FormsAuthenticationService(),
			new AccountMembershipService()) {
		}

		public AccountController(
			IModuleRepository moduleRepo,
			IFormsAuthentication formsAuth,
			IMembershipService service)
			: base(moduleRepo, service) {

			FormsAuth = formsAuth;
		}

		public IFormsAuthentication FormsAuth { get; private set; }

		[Authorize]
		public ActionResult Index() {
			return RedirectToAction("Login");
		}

		public ActionResult LogOn() {
			return RedirectToAction("Login");
		}

		public ActionResult Login() {
			var subdomain = Request.Url.GetSubDomain();

			if (!String.IsNullOrWhiteSpace(subdomain) && subdomain != "www") {
				var acct = ModRepository.GetAccountByName(subdomain);

				if (acct == null) {
					ModelState.AddModelError("_FORM", "Conta não encontrada.");
					return View("Login");
				}

				if (!String.IsNullOrWhiteSpace(acct.Logo)) {
					ViewData["accountlogo"] = Url.Content(acct.Logo);
				}
			}

			if (TempData["_FORM"] != null) {
				ModelState.AddModelError("_FORM", TempData["_FORM"].ToString());
			}
			
			return View("Login");
		}

		[HttpPost]
		public ActionResult OpenId(FormCollection form) {
			string url = "https://rpxnow.com/api/v2/auth_info";
			string apiKey = "510a0b6164005bbda028dc3a893d276dbd102b1b";
			string token = form["token"];

			var clnt = new WebClient();
			clnt.QueryString.Add("apiKey", apiKey);
			clnt.QueryString.Add("token", token);

			string jsonResult = null;

			try {
				jsonResult = clnt.DownloadString(url);
			}
			catch (Exception) {
				TempData["Message"] = "Falha ao efetuar o login. Por favor, tente novamente.";
				return RedirectToAction("Login");
			}

			var jss = new JavaScriptSerializer();
			var profileData = jss.Deserialize<OpenIdResult>(jsonResult);

			if (profileData.stat != "ok") {
				TempData["Message"] = "Falha ao efetuar o login";
				return RedirectToAction("Login");
			}

			var username = profileData.profile.identifier;

			if (Membership.GetUser(username) == null) {
				MembershipCreateStatus membershipCreateStatus;

				MembershipUser user = Membership.CreateUser(
					username,
					WebTools.GetRandomText(),
					profileData.profile.email,
					"Esta é uma conta do tipo OpenID. Você deve utilizar seu OpenID para se logar.",
					WebTools.GetRandomText(),
					true,
					out membershipCreateStatus);

				if (membershipCreateStatus != MembershipCreateStatus.Success) {	// failed
					TempData["_FORM"] = ErrorCodeToString(membershipCreateStatus);

					return RedirectToAction("Login");
				}

				user.Comment = profileData.profile.preferredUsername;

				Membership.UpdateUser(user);
			}

			var theUser = Membership.GetUser(username);
			if (theUser != null) {
				TempData["Message"] = "Bem vindo(a), " + theUser.UserName + ".";
			}

			FormsAuthentication.RedirectFromLoginPage(username, false);

			string returnUrl = Request.Form["returnUrl"];

			if (!String.IsNullOrEmpty(returnUrl) &&
				!returnUrl.EndsWith("Login")) {
				return Redirect(returnUrl);
			}

			return RedirectToAction("Index", "Home");
		}

		[HttpPost]
		public ActionResult Login(string username, string password, bool rememberMe, string returnUrl) {
			if (!ValidateLogOn(username, password)) {
				return Login();
			}

			// Make sure we have the username with the right capitalization
			// since we do case sensitive checks for OpenID Claimed Identifiers later.
			username = this.MembershipService.GetCanonicalUsername(username);

			// if the user is logging in using the pattern http://<accountname>.dovercms.com,
			// we'll check 
			var subdomain = Request.Url.GetSubDomain();

			if (!String.IsNullOrWhiteSpace(subdomain)) {
				var acct = ModRepository.GetAccountByName(subdomain);
				var user = acct.Users.FirstOrDefault(a => a.UserName == username);

				if (user == null) {
					ModelState.AddModelError("_FORM", "Nome de usuário ou senha incorreta.");
					return Login();
				}
			}
			
			FormsAuth.SignIn(username, rememberMe);

			TempData["Message"] = "Bem vindo(a), " + username + ".";

			if (!String.IsNullOrEmpty(returnUrl)) {
				return Redirect(returnUrl);
			}

			return RedirectToAction("Index", "Home");
		}

		[Authorize]
		public ActionResult LogOff() {
			FormsAuth.SignOut();

			return RedirectToAction("Login");
		}

		[Authorize]
		public ActionResult Edit(string id) {
			MembershipUser user = Membership.GetUser(id);
			UserProfileManager pMgr = new UserProfileManager(user);

			var userModules = ModRepository.GetUserModules((Guid)user.ProviderUserKey);
			var allModules = new List<IModule>();//ModRepository.GetAllStaticModules();

			EditUserViewModel viewModel = new EditUserViewModel() {
				UserId = (Guid)user.ProviderUserKey,
				Email = user.Email,
				UserProfile = pMgr.UserProfile,
				UserModules = userModules,
				AllModules = allModules.Where(module => userModules.FirstOrDefault(mod => mod.Id == module.Id) == null)
			};

			// add the list with all available database types
			// TODO: Move this code to the profile-related namespace
			List<object> databaseTypes = new List<object>();

			return View(viewModel);
		}

		public ActionResult Payment() {
			return View();
		}

		public ActionResult ThankYou() {
			return View();
		}

		public ActionResult Signup() {
			return View();
		}

		[Authorize]
		public ActionResult Details(string _sUserName) {
			return View();
		}

		[Authorize(Roles = "administrators, sysadmin")]
		public ActionResult List() {
			var model = new List<KeyValuePair<string, string>>();

			// sysadmin can see all system users
			if (Roles.IsUserInRole("sysadmin")) {
				foreach (var user in ModRepository.GetAllUsers()) {
					var sb = new StringBuilder();
					foreach(var acc in user.Accounts.Select(a => a.Name)) {
						sb.Append(acc + ", ");
					}
					model.Add(new KeyValuePair<string, string>(
						sb.ToString().TrimEnd(", ".ToCharArray()),
						user.UserName));
				}
			}
			else if(Roles.IsUserInRole("administrators")) {
				var userId = (Guid)Membership.GetUser().ProviderUserKey;
				var accounts = ModRepository.GetUserAccounts(userId);

				foreach (var acct in accounts) {
					foreach (var user in ModRepository.GetAccountUsers(acct.Id)) {
						model.Add(new KeyValuePair<string, string>(
							acct.Name,
							user.UserName));
					}
				}
			}

			return View(model);
		}

		[HttpPost, Authorize(Roles = "administrators, sysadmin")]
		public ActionResult DeleteAccount(int id) {
			try {
				ModRepository.DeleteAccount(id);
				ModRepository.Save();
			}
			catch (Exception e) {
				TempData["Message"] = "Erro ao remover a conta: " + e.Message;
				return RedirectToAction("ListAccounts");
			}

			TempData["Message"] = "Conta removida com sucesso.";
			return RedirectToAction("ListAccounts");
		}

		[Authorize(Roles = "administrators, sysadmin")]
		public ActionResult CreateAccount() {
			return View();
		}

		[HttpPost, Authorize(Roles = "administrators, sysadmin")]
		public ActionResult CreateAccount(AccountViewModel acct, string userName, string email, string password, string confirmPassword) {
			if (!ModelState.IsValid) {
				return View(acct);
			}

			var dbAcct = new Account {
				Name = acct.Name,
				SubdomainName = acct.Subdomain,
				Plan = ModRepository.GetPlanByName("Unlimited")
			};

			if (Request.Files.Count > 0 &&
				Request.Files[0].ContentLength > 0) {

				var user = Membership.GetUser() as UACUser;
				var imgInfo = user.SaveImage(Request.Files[0]);
				dbAcct.Logo = imgInfo.FullRelativePath;
			}

			if (!CreateNewUser(userName, email, password, confirmPassword)) {
				return View(acct);
			}

			var createdUser = ModRepository.GetUserByName(userName);

			dbAcct.Users.Add(createdUser);
			
			ModRepository.AddAccount(dbAcct);
			ModRepository.Save();

			TempData["Message"] = "Conta criada com sucesso!";
			return RedirectToAction("ListAccounts");
		}

		[Authorize(Roles = "administrators, sysadmin")]
		public ActionResult ListAccounts() {
			var model = new Dictionary<int, string>();

			// sysadmin can see all system users
			if (Roles.IsUserInRole("sysadmin")) {
				foreach (var acct in ModRepository.GetAllAccounts()) {
					model.Add(acct.Id, acct.Name);
				}
			}
			else if (Roles.IsUserInRole("administrators")) {
				var userId = (Guid)Membership.GetUser().ProviderUserKey;
				var accounts = ModRepository.GetUserAccounts(userId);

				foreach (var acct in accounts) {
					model.Add(acct.Id, acct.Name);
				}
			}

			return View(model);
		}

		[Authorize(Roles = "administrators, sysadmin")]
		public ActionResult EditAccount(string id) {
			Account acct = null;

			if (Roles.IsUserInRole("sysadmin")) {
				acct = ModRepository.GetAccountByName(id);
			}
			else if (Roles.IsUserInRole("administrators")) {
				var userId = (Guid)Membership.GetUser().ProviderUserKey;
				acct = ModRepository.GetUserAccounts(userId).FirstOrDefault(acc => acc.SubdomainName == id);
			}

			if (acct == null) {
				TempData["Message"] = "Conta não encontrada.";
				return RedirectToAction("Index", "Home");
			}

			return View(new AccountViewModel {
				Id = acct.Id,
				Logo = acct.Logo,
				Name = acct.Name,
				Subdomain = acct.SubdomainName,
				Users = acct.Users.Select(u => u.UserName).ToList()
			});
		}

		[HttpPost, Authorize(Roles = "administrators, sysadmin")]
		public ActionResult EditAccount(AccountViewModel acct) {
			if (!ModelState.IsValid) {
				return View(acct);
			}
			
			var dbAcct = ModRepository.GetAccountById(acct.Id);

			if (dbAcct == null) {
				TempData["Message"] = "Conta não encontrada.";
				return RedirectToAction("Index", "Home");
			}

			dbAcct.Name = acct.Name;
			dbAcct.SubdomainName = acct.Subdomain;
			
			if (Request.Files.Count > 0 &&
				Request.Files[0].ContentLength > 0) {
				var user = Membership.GetUser() as UACUser;
				var imgInfo = user.SaveImage(Request.Files[0]);
				dbAcct.Logo = imgInfo.FullRelativePath;
			}

			ModRepository.Save();

			TempData["Message"] = "Dados salvos com sucesso!";
			return RedirectToAction("ListAccounts");
		}

		/// <summary>
		/// Return current user's profile data
		/// </summary>
		/// <returns></returns>
		[Authorize]
		public ActionResult Profile() {
			MembershipUser user = Membership.GetUser();

			if (user == null) {
				RedirectToAction("Login");
			}

			UserProfileViewModel viewModel = new UserProfileViewModel() {
				AccountProperties = new UserProfileManager(user).UserProfile.Properties,
				UserId = (Guid)user.ProviderUserKey,
				Email = user.Email,
			};

			return View(viewModel);
		}

		[HttpPost, Authorize]
		public ActionResult Profile(FormCollection _vars) {
			var user = Membership.GetUser(Guid.Parse(_vars["UserGuid"]));
			var prfMgr = new UserProfileManager(user);

			UserProfileViewModel viewModel = new UserProfileViewModel() {
				AccountProperties = new UserProfileManager(user).UserProfile.Properties,
				UserId = (Guid)user.ProviderUserKey,
				Email = user.Email,
			};

			user.Email = _vars["Email"];

			string currPwd = _vars["CurrentPassword"];
			string newPwd = _vars["NewPassword"];
			string newPwdConfirmation = _vars["NewPasswordConfirmation"];

			if (!string.IsNullOrWhiteSpace(currPwd)) {
				// attempt to change the user's password
				if (string.IsNullOrWhiteSpace(newPwd)) {
					ModelState.AddModelError("NewPassword", "Você deve digitar a nova senha.");
					return View(viewModel);
				}
				if (string.IsNullOrWhiteSpace(newPwdConfirmation)) {
					ModelState.AddModelError("NewPassword", "Você deve digitar a confirmação da nova senha.");
					return View(viewModel);
				}
				if (newPwdConfirmation != newPwd) {
					ModelState.AddModelError("NewPassword", "A nova senha e a confirmação devem ser iguais.");
					ModelState.AddModelError("NewPasswordConfirmation", "A nova senha e a confirmação devem ser iguais.");
					return View(viewModel);
				}

				if (!user.ChangePassword(currPwd, newPwd)) {
					ModelState.AddModelError("CurrentPassword", "Não foi possível trocar sua senha. Verifique sua senha atual e tente novamente.");
					return View(viewModel);
				}
			}

			if (_vars["Administrator"] != null) {
				if (!Roles.IsUserInRole(user.UserName, "administrators")) {
					Roles.AddUserToRole(user.UserName, "administrators");
				}
			}
			else if (Roles.IsUserInRole(user.UserName, "administrators")) {
				Roles.RemoveUserFromRole(user.UserName, "administrators");
			}

			_vars.Remove("UserGuid");
			_vars.Remove("Email");
			_vars.Remove("CurrentPassword");
			_vars.Remove("NewPassword");
			_vars.Remove("NewPasswordConfirmation");
			_vars.Remove("Administrator");

			foreach (string key in _vars.Keys) {
				prfMgr.SetUserProfileProperty(key, _vars[key]);
			}

			try {
				Membership.UpdateUser(user);
			}
			catch (ProviderException e) {
				ModelState.AddModelError("Email", "Ocorreu um erro atualizar seus dados. " + e.Message);
				return View(viewModel);
			}

			TempData["Message"] = "Dados salvos com sucesso.";
			return RedirectToAction("Index", "Home");
		}

		public ActionResult ResetPassword() {
			return View();
		}

		[HttpPost]
		public ActionResult ResetPassword(string username) {
			if (String.IsNullOrWhiteSpace(username)) {
				ModelState.AddModelError("username", "Nome de usuário inválido");
				return View();
			}

			var user = MembershipService.GetUser(username);

			if (user == null) {
				ModelState.AddModelError("username", "Nome de usuário inválido");
				return View();
			}

			try {
				var newPwd = MembershipService.ResetPassword(user.UserName, null);
				var body = "Sua nova senha é " + newPwd;
				
				MailMessage msg = new MailMessage("dovercms@dovercms.com", user.Email, "Esqueci minha senha", body);
				SmtpClient smtp = new SmtpClient();
				smtp.Credentials = new NetworkCredential("dover@dovercms.com", "uac@dmin");
				smtp.Send(msg);
			}
			catch (Exception e) {
				ModelState.AddModelError("username", e.Message);
				return View();
			}

			TempData["Message"] = "Sua nova senha foi enviada para o seu email cadastrado.";

			return RedirectToAction("Login");
		}

		public ActionResult Register() {
			ViewData["PasswordLength"] = MembershipService.MinPasswordLength;

			return View();
		}

		[HttpPost, Authorize(Roles = "administrators, sysadmin")]
		public ActionResult Delete(string id) {
			try {
				// make sure the user is authorized to delete the other user
				var user = Membership.GetUser(id) as UACUser;
				if (!Membership.DeleteUser(user.ActualUserName, true)) {
					TempData["Message"] = "Ocorreu um erro ao remover o usuário.";
					return RedirectToAction("List");
				}
			}
			catch (Exception e) {
				TempData["Message"] = "Ocorreu um erro ao remover o usuário: " + e.Message;
				return RedirectToAction("List");
			}

			TempData["Message"] = "Usuário removido com sucesso.";
			return RedirectToAction("List");
		}

		[HttpPost]
		public ActionResult Register(string userName, string email, string password, string confirmPassword) {
			if (!CreateNewUser(userName, email, password, confirmPassword)) {
				return View();
			}

			var acctName = RouteData.Values["account"] as string;

			// associate the created user with the current account
			if (!String.IsNullOrWhiteSpace(acctName)) {
				var createdUser = ModRepository.GetUserByName(userName);
				var acct = ModRepository.GetAccountByName(acctName);
				
				acct.Users.Add(createdUser);

				ModRepository.Save();
			}
			
			TempData["Message"] = "Usuário " + userName + " criado com sucesso.";
			return RedirectToAction("Index", "Home");
		}

		/// <summary>
		/// Creates the new user.
		/// </summary>
		/// <param name="userName">Name of the user.</param>
		/// <param name="email">The email.</param>
		/// <param name="password">The password.</param>
		/// <param name="confirmPassword">The password confirmation.</param>
		/// <returns>Whether the user was successfully created or not</returns>
		private bool CreateNewUser(string userName, string email, string password, string confirmPassword) {
			ViewData["PasswordLength"] = MembershipService.MinPasswordLength;
			

			if (!ValidateRegistration(userName, email, password, confirmPassword)) {
				return false;
			}

			// Attempt to register the user
			MembershipCreateStatus createStatus = MembershipService.CreateUser(userName, password, email);

			bool success = (createStatus == MembershipCreateStatus.Success);

			if (!success) {
				ModelState.AddModelError("_FORM", ErrorCodeToString(createStatus));
			}

			return success;
		}

		[Authorize]
		public ActionResult ChangePassword() {

			ViewData["PasswordLength"] = MembershipService.MinPasswordLength;

			return View();
		}

		[HttpPost]
		[Authorize]
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Exceptions result in password not being changed.")]
		public ActionResult ChangePassword(string currentPassword, string newPassword, string confirmPassword) {
			ViewData["PasswordLength"] = MembershipService.MinPasswordLength;

			if (!ValidateChangePassword(currentPassword, newPassword, confirmPassword)) {
				return View();
			}

			try {
				if (MembershipService.ChangePassword(User.Identity.Name, currentPassword, newPassword)) {
					return RedirectToAction("ChangePasswordSuccess");
				}
				else {
					ModelState.AddModelError("_FORM", "The current password is incorrect or the new password is invalid.");
					return View();
				}
			}
			catch {
				ModelState.AddModelError("_FORM", "The current password is incorrect or the new password is invalid.");
				return View();
			}
		}

		[Authorize]
		public ActionResult ChangePasswordSuccess() {

			return View();
		}

		protected override void OnActionExecuting(ActionExecutingContext filterContext) {
			if (filterContext.HttpContext.User.Identity is WindowsIdentity) {
				throw new InvalidOperationException("Windows authentication is not supported.");
			}
		}

		#region Validation Methods

		private bool ValidateChangePassword(string currentPassword, string newPassword, string confirmPassword) {
			if (String.IsNullOrEmpty(currentPassword)) {
				ModelState.AddModelError("currentPassword", "Você deve digitar a senha atual.");
			}
			if (newPassword == null || newPassword.Length < MembershipService.MinPasswordLength) {
				ModelState.AddModelError("newPassword",
					String.Format(CultureInfo.CurrentCulture,
						 "Você deve digitar uma senha de {0} ou mais caracteres.",
						 MembershipService.MinPasswordLength));
			}

			if (!String.Equals(newPassword, confirmPassword, StringComparison.Ordinal)) {
				ModelState.AddModelError("_FORM", "The new password and confirmation password do not match.");
			}

			return ModelState.IsValid;
		}

		private bool ValidateLogOn(string userName, string password) {
			if (String.IsNullOrEmpty(userName)) {
				ModelState.AddModelError("username", "Você deve preencher o nome de usuário.");
			}
			if (String.IsNullOrEmpty(password)) {
				ModelState.AddModelError("password", "Você deve preencher a senha.");
			}
			if (!MembershipService.ValidateUser(userName, password)) {
				ModelState.AddModelError("_FORM", "O nome de usuário ou senha digitada está incorreta.");
			}

			return ModelState.IsValid;
		}

		private bool ValidateRegistration(string userName, string email, string password, string confirmPassword) {
			if (String.IsNullOrEmpty(userName)) {
				ModelState.AddModelError("username", "Você deve preencher o nome de usuário.");
			}
			if (String.IsNullOrEmpty(email)) {
				ModelState.AddModelError("email", "Você deve fornecer um endereço de email.");
			}
			if (password == null || password.Length < MembershipService.MinPasswordLength) {
				ModelState.AddModelError("password",
					String.Format(CultureInfo.CurrentCulture,
						 "Você deve digitar uma senha de {0} ou mais caracteres.",
						 MembershipService.MinPasswordLength));
			}
			if (!String.Equals(password, confirmPassword, StringComparison.Ordinal)) {
				ModelState.AddModelError("_FORM", "A senha e confirmação sevem ser iguais.");
			}
			return ModelState.IsValid;
		}

		private static string ErrorCodeToString(MembershipCreateStatus createStatus) {
			// See http://msdn.microsoft.com/en-us/library/system.web.security.membershipcreatestatus.aspx for
			// a full list of status codes.
			switch (createStatus) {
				case MembershipCreateStatus.DuplicateUserName:
					return "O nome de usuário escolhido já existe. Por favor, escolha outro.";

				case MembershipCreateStatus.DuplicateEmail:
					return "Já existe um usuário para o endereço de email escolhido. Por favor, escolha um email diferente.";

				case MembershipCreateStatus.InvalidPassword:
					return "A senha digitada é inválida. Por favor, digite uma senha válida.";

				case MembershipCreateStatus.InvalidEmail:
					return "O endereço de email é inválido. Por favor, verifique e tente novamente.";

				case MembershipCreateStatus.InvalidAnswer:
					return "The password retrieval answer provided is invalid. Please check the value and try again.";

				case MembershipCreateStatus.InvalidQuestion:
					return "The password retrieval question provided is invalid. Please check the value and try again.";

				case MembershipCreateStatus.InvalidUserName:
					return "O nome de usuário digitado está incorreto. Por favor, verifique e tente novamente.";

				case MembershipCreateStatus.ProviderError:
					return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

				case MembershipCreateStatus.UserRejected:
					return "O pedido de criação de usuário foi cancelada. Por favor, verifique seus dados e tente novamente. Se o problema persistir, entre em contato com o administrador do sistema.";

				default:
					return "Ocorreu um erro desconhecido. Por favor, verifique seu dados e tente novamente. Se o problema persistir, entre em contato com o administrador do sistema.";
			}
		}
		#endregion
	}

	// The FormsAuthentication type is sealed and contains static members, so it is difficult to
	// unit test code that calls its members. The interface and helper class below demonstrate
	// how to create an abstract wrapper around such a type in order to make the AccountController
	// code unit testable.

	public interface IFormsAuthentication {
		void SignIn(string userName, bool createPersistentCookie);
		void SignOut();
	}

	public class FormsAuthenticationService : IFormsAuthentication {
		public void SignIn(string userName, bool createPersistentCookie) {
			FormsAuthentication.SetAuthCookie(userName, createPersistentCookie);
		}
		public void SignOut() {
			FormsAuthentication.SignOut();
		}
	}
}