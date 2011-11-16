using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Schema;
using System.Xml;
using Com.Dover.Web.Models.Converters;
using Com.Dover.Helpers;
using System.Text;
using Com.Dover.Attributes;

namespace Com.Dover.Web.Models.DataTypes {
	
	[Serializable]
	[FieldValueConverter(ConverterType = typeof(CheckBoxListConverter))]
	public class CheckBoxList : List<CheckBoxItem>, IFieldDataType {

		#region IXmlSerializable Members

		public XmlSchema GetSchema() {
			return null;
		}

		public void ReadXml(XmlReader reader) {
			reader.ReadStartElement("CheckBoxList");
			while (reader.IsStartElement()) {
				var item = new CheckBoxItem();
				item.ReadXml(reader);
				this.Add(item);
			}
			if (reader.NodeType == XmlNodeType.EndElement) {
				reader.ReadEndElement();
			}
		}

		public void WriteXml(XmlWriter writer) {
			foreach (var item in this.Where(i => i.IsChecked)) {
				writer.WriteStartElement("CheckItem");
				item.WriteXml(writer);
				writer.WriteEndElement();
			}
		}

		#endregion

		public override string ToString() {
			var sb = new StringBuilder();
			foreach (var item in this.Where(i => i.IsChecked).OrderBy(i => i.Key)) {
				sb.Append(item + ", ");
			}
			return sb.ToString().TrimEnd(", ".ToCharArray());
		}
	}
}