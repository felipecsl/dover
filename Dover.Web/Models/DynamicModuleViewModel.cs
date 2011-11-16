using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Web.Mvc;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml;

namespace Com.Dover.Web.Models {
    public class DynamicModuleViewModel : IXmlSerializable {
        public DynamicModuleViewModel() {
            this.Fields = new DynamicModuleFieldList();
            this.ID = -1;	// initialize with an invalid Id
        }

        [HiddenInput(DisplayValue = false)]
        [UIHint("HiddenInput")]
        public int ID { get; set; }

        public DynamicModuleFieldList Fields { get; set; }

        [HiddenInput(DisplayValue = false)]
        [UIHint("HiddenInput")]
        public string ModuleName { get; set; }

        public string DisplayName { get; set; }

        #region IXmlSerializable Members

        public XmlSchema GetSchema() {
            return null;
        }

        public void ReadXml(XmlReader reader) {
            throw new NotImplementedException();
        }

        public void WriteXml(XmlWriter writer) {
            writer.WriteStartElement("ID");
            writer.WriteValue(ID);
            writer.WriteEndElement();
            Fields.WriteXml(writer);
        }

        #endregion
    }
}