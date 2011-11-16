using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Com.Dover.Modules;
using System.Data.EntityClient;

namespace Com.Dover.Tests {
	[TestClass]
	public class AccountTests {

		[TestMethod]
		public void TestAddUserToAccount() {
			var conn = new EntityConnection(SerializationTests.ConnectionString);
			var repo = new ModuleRepository(conn);
			var account = new Account {
				Name = "DBServer",
				SubdomainName = "dbserver"
			};
			var user = repo.GetUserByName("dbserver");
			account.Users.Add(user);
			repo.AddAccount(account);
			repo.Save();

			var newAcct = repo.GetAccountByName("dbserver");

			Assert.IsNotNull(newAcct);

			var newUser = newAcct.Users.FirstOrDefault(u => u.UserId == user.UserId);

			Assert.IsNotNull(newUser);
		}
	}
}
