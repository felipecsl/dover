using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Xml;
using System.Xml.Schema;

namespace Com.Dover.Web.Models.DataTypes {
    [Serializable]
    public class File : IFieldDataType {

        public string FilePath {
            get { return this.filePath; }
            set { this.filePath = value; }
        }
        private string filePath;

        public string AbsolutePath {
            get {
				if (!String.IsNullOrWhiteSpace(FilePath)) {
					if (FilePath.StartsWith("~")) {

						return String.Format("{0}{1}/{2}",
							DoverApplication.Scheme,
							DoverApplication.DomainName,
							FilePath.TrimStart("~".ToCharArray()));
					}
				}
				return FilePath;
            }
        }

        public string FileName {
			get { return FilePath; }
        }

        #region IXmlSerializable Members

        public XmlSchema GetSchema() {
			throw null;
        }

        public void ReadXml(XmlReader reader) {
			if (reader.IsEmptyElement) {
				reader.Read();
				return;
			}
			reader.ReadStartElement("File");
			FilePath = reader.ReadString();
			reader.ReadEndElement();
        }

        public void WriteXml(XmlWriter writer) {
            writer.WriteValue(this.ToString());
        }

        #endregion

		public override string ToString() {
			return AbsolutePath;
		}
    }
}