using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Com.Dover.Web.Models.DataTypes;
using Com.Dover.Modules;

namespace Com.Dover.Web.Models.Converters {
	public class HtmlTextFieldValueConverter : DefaultFieldValueConverter {
		public override string Serialize(DynamicModuleField _obj, ConversionContext _context = null) {
			if (_context == null) {
				throw new ArgumentNullException("_context");
			}
			if (_obj == null) {
				throw new ArgumentNullException("_obj");
			}

			var data = _obj.Data as HtmlText;

			if (data != null && !String.IsNullOrWhiteSpace(data.Text)) {
				data.Text = HttpUtility.HtmlDecode(data.Text);
				_obj.Data = data;
			}

			return base.Serialize(_obj, _context);
		}

		/*public override object Deserialize(Cell _data, ConversionContext _context = null) {
			object data = base.Deserialize(_data, _context);

			if (_context == null) {
				throw new ArgumentNullException("_context");
			}

			HtmlText htmlText;

			if (data == null) {
				htmlText = new HtmlText();
			}
			else {
				htmlText = data as HtmlText;

				if (!String.IsNullOrWhiteSpace(htmlText.Text)) {
					htmlText.Text = HttpUtility.HtmlEncode(htmlText.Text);
				}
			}

			return htmlText;
		}*/
	}
}