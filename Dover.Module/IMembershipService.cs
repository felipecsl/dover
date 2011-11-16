using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;

namespace Com.Dover.Modules {
    public interface IMembershipService {
		int MinPasswordLength { get; }

		bool IsUserInRole(string username, string roleName);
		bool ChangePassword(string userName, string oldPassword, string newPassword);
		bool ValidateUser(string userName, string password);
		
		string GetCanonicalUsername(string userName);
		string ResetPassword(string username, string answer);

		MembershipUser GetUser();
		MembershipUser GetUser(string username);
		
		MembershipCreateStatus CreateUser(string userName, string password, string email);
    }
}
