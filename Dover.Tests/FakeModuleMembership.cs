using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;
using Com.Dover.Helpers;
using Com.Dover.Modules;

namespace Com.Dover.Tests {
	class FakeModuleMembership : IMembershipService {
        public static Guid FakeUserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        static FakeModuleMembership() {
            FakeUserId = Guid.NewGuid();
        }
        
        public FakeModuleMembership(
            string _username = "John Doe", 
            string _password = "john@doe.com") {

            UserName = _username;
            Password = _password;
        }

		#region IMembershipService Members

		public MembershipUser GetUser() {
            return new MembershipUser(
                "UACMembershipProvider",
                UserName,
                FakeUserId,
                Password,
                "What is my favourite color?",
                null,
                true,
                false,
                DateTime.MinValue,
                DateTime.MinValue,
                DateTime.MinValue,
                DateTime.MinValue,
                DateTime.MinValue);

        }

		public bool IsUserInRole(string username, string roleName) {
			if (username == "Fake Admin" && roleName == "administrators") {
				return true;
			}
			return false;
		}

		public int MinPasswordLength {
			get { throw new NotImplementedException(); }
		}

		public bool ChangePassword(string userName, string oldPassword, string newPassword) {
			throw new NotImplementedException();
		}

		public bool ValidateUser(string userName, string password) {
			throw new NotImplementedException();
		}

		public string GetCanonicalUsername(string userName) {
			throw new NotImplementedException();
		}

		public string ResetPassword(string username, string answer) {
			throw new NotImplementedException();
		}

		public MembershipUser GetUser(string username) {
			throw new NotImplementedException();
		}

		public MembershipCreateStatus CreateUser(string userName, string password, string email) {
			throw new NotImplementedException();
		}

		#endregion
	}
}
