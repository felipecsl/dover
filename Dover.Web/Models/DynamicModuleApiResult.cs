using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml;
using Com.Dover.Modules;

namespace Com.Dover.Web.Models {
	public class DynamicModuleApiResult : IXmlSerializable {
        public DynamicModuleApiResult() {
            this.Rows = new List<DynamicModuleViewModel>();
        }

        public string ModuleName { get; set; }
        public int ModuleType { get; set; }
		public int ModuleId { get; set; }

        public List<DynamicModuleViewModel> Rows { get; set; }

        #region IXmlSerializable Members

        public XmlSchema GetSchema() {
            return null;
        }

        public void ReadXml(XmlReader reader) {
			ModuleName = reader.Name;
			reader.ReadStartElement();
			ModuleId = reader.ReadElementContentAsInt("ModuleId", String.Empty);
        }

        public void WriteXml(XmlWriter writer) {
			writer.WriteStartElement("ModuleId");
			writer.WriteValue(ModuleId);
			writer.WriteEndElement();
			
			if(ModuleType == (int)Com.Dover.Modules.ModuleType.SingleEntry) {
                if(Rows.Count > 0) {
                    Rows[0].WriteXml(writer);
                    return;
                }
            }

            foreach(var row in Rows) {
                writer.WriteStartElement(ModuleName.TrimEnd("s".ToCharArray()));
                row.WriteXml(writer);
                writer.WriteEndElement();
            }
        }

        #endregion

        public override string ToString() {
            return this.ModuleName;
        }
    }
}