using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.Globalization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Security;
using Com.Dover.Attributes;
using Com.Dover.Web.Models.Converters;

namespace Com.Dover.Web.Models.DataTypes {
	[TypeConverter(typeof(HtmlTextTypeConverter))]
	[Serializable]
	[FieldValueConverter(ConverterType = typeof(HtmlTextFieldValueConverter))]
	public class HtmlText : IFieldDataType {
		public string Text { get; set; }

		public override string ToString() {
			return this.Text;
		}

		#region IXmlSerializable Members

		public XmlSchema GetSchema() {
			return null;
		}

		public void ReadXml(XmlReader reader) {
			if (reader.IsEmptyElement) {
				reader.Read();
				return;
			}
			reader.ReadStartElement("HtmlText");
			Text = reader.ReadString();
			reader.ReadEndElement();
		}

		public void WriteXml(XmlWriter writer) {
			string snippet = "src=\"{0}/ckfinder/userfiles";

			if (HttpContext.Current != null) {

				var currRequest = HttpContext.Current.Request;

				if (currRequest != null && Text != null) {

					string fixedPath = String.Format(snippet, currRequest.Url.Scheme + "://" + currRequest.Url.Host);
					writer.WriteValue(Text.Replace("src=\"/ckfinder/userfiles", fixedPath));
				}
			}
			else {
				writer.WriteValue(Text);
			}
		}

		#endregion
	}

	public class HtmlTextTypeConverter : TypeConverter {
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			return sourceType == typeof(string)
				? true
				: base.CanConvertFrom(context, sourceType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
			return (value is string)
				? new HtmlText { Text = (string)value }
				: null;
		}

	}
}