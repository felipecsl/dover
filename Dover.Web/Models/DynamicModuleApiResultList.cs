using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Com.Dover.Web.Models;
using System.Xml.Serialization;
using System.Xml.Schema;

namespace Com.Dover.Web.Models {
	
	[Serializable]
	public class DynamicModuleApiResultList : List<DynamicModuleApiResult>, IXmlSerializable {
		
		#region IXmlSerializable Members

		public XmlSchema GetSchema() {
			return null;
		}

		public void ReadXml(System.Xml.XmlReader reader) {
			throw new NotImplementedException();
		}

		public void WriteXml(System.Xml.XmlWriter writer) {
			foreach (var result in this) {
				writer.WriteStartElement(result.ToString());
				result.WriteXml(writer);
				writer.WriteEndElement();
			}
		}

		#endregion

		public override string ToString() {
			// Root level for dynamic module api result list
			return "Results";
		}

	}
}