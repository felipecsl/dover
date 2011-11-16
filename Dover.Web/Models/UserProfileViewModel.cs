using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Com.Dover.Profile;
using System.Web.Mvc;

namespace Com.Dover.Web.Models {
	public class UserProfileViewModel {
		public Guid UserId { get; set; }
		public List<ProfileProperty> AccountProperties { get; set; }
		public string Email { get; set; }
	}
}
