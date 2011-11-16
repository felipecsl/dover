using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Com.Dover.Web.Models.DataTypes;
using Br.Com.Quavio.Tools.Web.Encryption;

namespace Com.Dover.Web.Models.Converters {
	public class PasswordConverter : DefaultFieldValueConverter {
		public override string Serialize(DynamicModuleField _obj, ConversionContext _context = null) {
			// For security reasons we cannot send the user's password in plain text to the browser.
			// We send a bogus text. Because of this, we need to check whether the user
			// has changed the password value or not.

			if (_context == null) {
				throw new ArgumentNullException("_context");
			}
			if (_obj == null) {
				throw new ArgumentNullException("_obj");
			}
			
			var data = _obj.Data as Password;

			// A Password field cannot be set to null or empty string by design
			if (data == null ||
				String.IsNullOrWhiteSpace(data.Value) || 
				data.Value == Password.BogusText) {

				// The user has not changed his password. 
				// Let's retrieve the original value from the database and store it again.
				var originalModel = new DynamicModuleField();

				originalModel.Data = Deserialize(_context.Cell, _context);

				_obj.Data = originalModel.Data;
			}
			else {
				// The user is setting a new password. We'll generate a hash for it
				var hash = new Hash(Hash.Provider.SHA1);
				data.Value = hash.Calculate(new Data(data.Value), new Data("|)0ver3ncrypt10n_k3y")).Base64;
			}

			return base.Serialize(_obj, _context);
		}
	}
}