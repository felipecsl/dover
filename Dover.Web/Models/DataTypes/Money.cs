using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Schema;
using System.Xml;
using System.ComponentModel;
using System.Globalization;

namespace Com.Dover.Web.Models.DataTypes {
	[Serializable]
	public class Money : IFieldDataType {
		public float? Value {
			get { return this._value; }
			set { this._value = value; }
		}
		private float? _value;

		public override string ToString() {
			return (this.Value != null)
				? this.Value.ToString()
				: String.Empty;
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
			reader.ReadStartElement("Money");
			float val;
			if (Single.TryParse(reader.ReadString(), out val)) {
				Value = val;
			}
			reader.ReadEndElement();
		}

		public void WriteXml(XmlWriter writer) {
			if (Value != null) {
				writer.WriteValue(Value.ToString());
			}
		}

		#endregion
	}
}