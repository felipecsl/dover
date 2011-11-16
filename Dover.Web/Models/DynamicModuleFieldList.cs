using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Schema;
using System.Xml;
using System.Xml.Serialization;

namespace Com.Dover.Web.Models {
	public class DynamicModuleFieldList : List<DynamicModuleField>, IXmlSerializable {
		#region IXmlSerializable Members

		public XmlSchema GetSchema() {
			return null;
		}

		public void ReadXml(XmlReader reader) {
			throw new NotImplementedException();
		}

		public void WriteXml(XmlWriter writer) {
			foreach (var item in this) {
				writer.WriteStartElement(item.PropertyName);
				item.WriteXml(writer);
				writer.WriteEndElement();
			}
		}

		#endregion
	}
}