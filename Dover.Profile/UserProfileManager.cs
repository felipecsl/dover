using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Profile;
using System.Web.Security;
using System.Configuration;
using System.Collections;

namespace Com.Dover.Profile {
	public class UserProfileManager {
		public UserProfileManager() {
		}

		public UserProfile UserProfile { get; private set; }

		public UserProfileManager(MembershipUser _user) {
			if (_user == null) {
				throw new ArgumentNullException("_user");
			}

			UserProfile newProfile = new UserProfile(_user.UserName);
			ProfileBase userProfile = ProfileBase.Create(_user.UserName);
			ProfileGroupBase addressGroup = userProfile.GetProfileGroup("Address");

			userProfile.Initialize(_user.UserName, true);

			newProfile.Properties.Add(new ProfileProperty("Nome", "Name", userProfile["Name"].ToString()));

			newProfile.Properties.Add(new ProfileProperty("Telefone", "Phone", addressGroup["Phone"].ToString()));
			newProfile.Properties.Add(new ProfileProperty("CEP", "CEP", addressGroup["CEP"].ToString()));
			newProfile.Properties.Add(new ProfileProperty("Endereço", "Street", addressGroup["Street"].ToString()));
			newProfile.Properties.Add(new ProfileProperty("Bairro", "Area", addressGroup["Area"].ToString()));
			newProfile.Properties.Add(new ProfileProperty("Estado", "State", addressGroup["State"].ToString()));
			newProfile.Properties.Add(new ProfileProperty("Cidade", "City", addressGroup["City"].ToString()));

			/*newProfile.Properties.Add(new ProfileProperty("FTP: Host", "FtpHost", ftpInfoGroup["FtpHost"].ToString()));
			newProfile.Properties.Add(new ProfileProperty("FTP: Usuário", "FtpUserName", ftpInfoGroup["FtpUserName"].ToString()));
			newProfile.Properties.Add(new ProfileProperty("FTP: Senha", "FtpPassword", ftpInfoGroup["FtpPassword"].ToString()));*/

			this.UserProfile = newProfile;
		}

		public string GetUserProfileProperty(string _sPropertyKey) {
			if (this.UserProfile == null) {
				return null;
			}

			ProfileProperty property = this.UserProfile.Properties.GetProperty(_sPropertyKey);

			return (property != null)
				? property.Value
				: null;
		}

		public void SetUserProfileProperty(string key, string value) {
			ProfileBase profileSettings = ProfileBase.Create(this.UserProfile.UserName, true);

			// TODO: Fix this crap!
			try {
				profileSettings[key] = value;
			}
			catch (SettingsPropertyNotFoundException) {
				ProfileGroupBase addressGroup = null;

				addressGroup = profileSettings.GetProfileGroup("Address");
				addressGroup[key] = value;
			}

			profileSettings.Save();
		}
	}
}
