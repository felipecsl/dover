using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Dover.Infrastructure {
	public class OpenIdResult {
		public string stat { get; set; }
		public OpenIdProfile profile { get; set; }
	}
	
	public class OpenIdProfile {
		public string verifiedEmail { get; set; }
		public string displayName { get; set; }
		public string preferredUsername { get; set; }
		public string url { get; set; }
		public string providerName { get; set; }
		public string identifier { get; set; }
		public string email { get; set; }
		public string photo { get; set; }
		public string birthday { get; set; }
		public string gender { get; set; }
		public string phoneNumber { get; set; }
		public OpenIdAddress address { get; set; }
		public OpenIdName name { get; set; }
	}

	public class OpenIdName {
		public string givenName { get; set; }
		public string familyName { get; set; }
		public string middleName { get; set; }
		public string formatted { get; set; }
	}

	public class OpenIdAddress {
		public string formatted { get; set; }
		public string streetAddress { get; set; }
		public string locality { get; set; }
		public string region { get; set; }
		public string postalCode { get; set; }
		public string country { get; set; }

	}
}
