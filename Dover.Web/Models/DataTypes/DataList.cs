using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections.Specialized;
using System.Xml.Schema;
using System.Xml;
using System.ComponentModel;
using System.Text;

namespace Com.Dover.Web.Models.DataTypes {
    [Serializable]
    public class DataList : IFieldDataType {

        public List<string> Items { 
            get {
                if(_items == null) {
                    _items = new List<string>();
                }
                return _items;
            }
            set { _items = value; }
        }
        private List<string> _items;

        #region IXmlSerializable Members

        public XmlSchema GetSchema() {
            throw new NotImplementedException();
        }

        public void ReadXml(XmlReader reader) {
			reader.ReadStartElement("DataList");
			string items = reader.ReadString();
			foreach (var i in items.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)) {
				this.Items.Add(i.Trim());
			}
			reader.ReadEndElement();
        }

        public void WriteXml(XmlWriter writer) {
            writer.WriteValue(this.ToString());
        }

        #endregion

		public override string ToString() {
			var sb = new StringBuilder();
			foreach (var item in Items) {
				sb.Append(item + ", ");
			}
			return sb.ToString().TrimEnd(", ".ToCharArray());
		}
    }
}