using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Schema;
using System.Xml;
using Com.Dover.Web.Models.Converters;
using Com.Dover.Helpers;
using System.Text;
using Br.Com.Quavio.Tools.Web;

namespace Com.Dover.Web.Models.DataTypes {
	[Serializable]
	public class CheckBoxItem : IFieldDataType {
		public string Key { get; set; }
		public string Value { get; set; }
		public bool IsChecked { get; set; }

		#region IXmlSerializable Members

		public XmlSchema GetSchema() {
			return null;
		}

		public void ReadXml(XmlReader reader) {
			if (reader.IsEmptyElement) {
				reader.Read();
				return;
			}

			if (reader.Name == "CheckItem") {
				Value = reader.ReadString();
				Key = Value.EscapeName();
				IsChecked = true;
				reader.ReadEndElement();
			}
		}

		public void WriteXml(XmlWriter writer) {
			writer.WriteValue(Value);
		}

		#endregion

		public override string ToString() {
			return Value;
		}
	}
}