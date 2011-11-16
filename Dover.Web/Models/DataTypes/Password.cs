using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Schema;
using System.Xml;
using System.ComponentModel;
using System.Globalization;
using Com.Dover.Helpers;
using Com.Dover.Web.Models.Converters;
using Com.Dover.Attributes;

namespace Com.Dover.Web.Models.DataTypes {
	[FieldValueConverter(ConverterType = typeof(PasswordConverter))]
	[TypeConverter(typeof(PasswordTypeConverter))]
	public class Password : IFieldDataType {
		public string Value { get; set; }

		public static string BogusText { get { return "********"; } }

		#region IXmlSerializable Members

		public XmlSchema GetSchema() {
			return null;
		}

		public void ReadXml(XmlReader reader) {
			if (reader.IsEmptyElement) {
				reader.Read();
				return;
			}
			reader.ReadStartElement("Password");
			Value = reader.ReadString();
			reader.ReadEndElement();
		}

		public void WriteXml(XmlWriter writer) {
			writer.WriteValue(this.ToString());
		}

		#endregion

		public override string ToString() {
			return Value;
		}

		public class PasswordTypeConverter : TypeConverter {
			public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
				return sourceType == typeof(string)
					? true
					: base.CanConvertFrom(context, sourceType);
			}

			public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
				return (value is string)
					? new Password { Value = (string)value }
					: null;
			}

		}
	}
}