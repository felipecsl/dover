using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.Xml;
using System.Xml.Schema;
using System.Globalization;
using System.Text;

namespace Com.Dover.Web.Models.DataTypes {
    [Serializable]
    public class ImageList : List<DbImage>, IFieldDataType {
        #region IXmlSerializable Members

        public XmlSchema GetSchema() {
            return null;
        }

        public void ReadXml(XmlReader reader) {
			reader.ReadStartElement("ImageList");
			while (reader.IsStartElement()) {
				var img = new DbImage();
				img.ReadXml(reader);
				this.Add(img);
			}
			reader.ReadEndElement();
        }

        public void WriteXml(XmlWriter writer) {
            foreach(var img in this.OrderBy(i => i.SortIndex)) {
                writer.WriteStartElement("Image");
                img.WriteXml(writer);
                writer.WriteEndElement();
            }
        }

        #endregion

		public override string ToString() {
			var sb = new StringBuilder();
			foreach (var item in this) {
				sb.Append(item + ", ");
			}
			return sb.ToString().TrimEnd(", ".ToCharArray());
		}
    }
}