using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.Globalization;
using System.Xml.Serialization;
using System.Web.Script.Serialization;
using Com.Dover.Controllers;
using System.Xml.Schema;
using System.Xml;
using Com.Dover.Modules;
using System.Web.Mvc;

namespace Com.Dover.Web.Models.DataTypes {
    [Serializable]
    public class DbImage : IFieldDataType {
        public string ImagePath {
            get { return this.imagePath; }
            set { this.imagePath = value; }
        }
        private string imagePath;

        public string AbsolutePath {
            get {
				if (!String.IsNullOrWhiteSpace(imagePath)) {
					if (imagePath.StartsWith("~")) {
						
						return String.Format("{0}{1}/{2}", 
							DoverApplication.Scheme, 
							DoverApplication.DomainName, 
							imagePath.TrimStart("~".ToCharArray()));
					}
				}
				return imagePath;
            }
        }

        public string Label { get; set; }
        public int SortIndex { get; set; }

        #region IXmlSerializable Members

        public XmlSchema GetSchema() {
            return null;
        }

        public void ReadXml(XmlReader reader) {
			if (reader.IsEmptyElement) {
				reader.Read();
				return;
			}
			if (reader.Name == "Image") {// Part of a ImageList
				reader.ReadStartElement("Image");
			}
			else {
				reader.ReadStartElement("DbImage");
			}
			imagePath = reader.ReadString();
			reader.ReadEndElement();
        }

        public void WriteXml(XmlWriter writer) {
            writer.WriteValue(AbsolutePath);
        }

		public override string ToString() {
			return AbsolutePath;
		}

        #endregion
    }
}