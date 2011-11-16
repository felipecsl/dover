using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;

namespace Com.Dover.Profile {
	public class UACMembershipProvider : SqlMembershipProvider {
		public override MembershipUser GetUser(object providerUserKey, bool userIsOnline) {
			var user = base.GetUser(providerUserKey, userIsOnline);

			return ConvertUser(user);
		}

		public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords) {
			var col = new MembershipUserCollection();

			foreach (MembershipUser user in base.GetAllUsers(pageIndex, pageSize, out totalRecords)) {
				col.Add(GetUser(user.UserName, false));
			}

			return col;
		}

		public override MembershipUser GetUser(string username, bool userIsOnline) {
			var user = base.GetUser(username, userIsOnline);

			return ConvertUser(user);
		}

		public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status) {
			if (String.IsNullOrWhiteSpace(email)) {
				email = Guid.NewGuid().ToString().Substring(0, 4) + "@dovercms.com";	// Generate random unique email
			}

			return base.CreateUser(username, password, email, passwordQuestion, passwordAnswer, isApproved, providerUserKey, out status);
		}

		private UACUser ConvertUser(MembershipUser user) {
			return user == null ? null : new UACUser(
				user.ProviderName,
				user.UserName,
				user.ProviderUserKey,
				user.Email,
				user.PasswordQuestion,
				user.Comment,
				user.IsApproved,
				user.IsLockedOut,
				user.CreationDate,
				user.LastLoginDate,
				user.LastActivityDate,
				user.LastPasswordChangedDate,
				user.LastLockoutDate);
		}
	}
}