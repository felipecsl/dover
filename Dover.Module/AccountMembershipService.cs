using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace Com.Dover.Modules {
	public class AccountMembershipService : IMembershipService {
        private MembershipProvider _provider;
		private RoleProvider _roleProvider;

		public AccountMembershipService()
			: this(Membership.Provider, Roles.Provider) {
		}

		public AccountMembershipService(MembershipProvider provider, RoleProvider roleProvider) {
			_provider = provider;
			_roleProvider = roleProvider;
		}

		public int MinPasswordLength {
			get {
				return _provider.MinRequiredPasswordLength;
			}
		}

		public bool ValidateUser(string userName, string password) {
			return _provider.ValidateUser(userName, password);
		}

		public string ResetPassword(string username, string answer) {
			return _provider.ResetPassword(username, answer);
		}

		public string GetCanonicalUsername(string userName) {
			var user = _provider.GetUser(userName, true);
			if (user != null) {
				return user.UserName;
			}

			return null;
		}

		public bool IsUserInRole(string username, string roleName) {
			return _roleProvider.IsUserInRole(username, roleName);
		}

		public MembershipUser GetUser() {
			return Membership.GetUser();
		}

		public MembershipUser GetUser(string username) {
			return _provider.GetUser(username, true);
		}

		public MembershipCreateStatus CreateUser(string userName, string password, string email) {
			MembershipCreateStatus status;
			_provider.CreateUser(userName, password, email, null, null, true, null, out status);
			return status;
		}

		public bool ChangePassword(string userName, string oldPassword, string newPassword) {
			MembershipUser currentUser = _provider.GetUser(userName, true /* userIsOnline */);
			return currentUser.ChangePassword(oldPassword, newPassword);
		}
    }
}