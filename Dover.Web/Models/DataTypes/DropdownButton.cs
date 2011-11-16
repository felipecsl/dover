using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Schema;
using System.Xml;
using Com.Dover.Helpers;
using Com.Dover.Web.Models.Converters;
using Com.Dover.Attributes;

namespace Com.Dover.Web.Models.DataTypes {

    [Serializable]
	[FieldValueConverter(ConverterType = typeof(DropdownButtonConverter))]
	public class DropdownButton : IFieldDataType {

		[NonSerialized]
		private IDictionary<string, string> dictValidValues;

        public IDictionary<string, string> ValidValues {
            get {
                if(dictValidValues == null) {
                    dictValidValues = new Dictionary<string, string>();
                }
                return dictValidValues;
            }
            set { dictValidValues = value; }
        }

		private string selectedValue;
		public string SelectedValue {
			get {

				if(selectedValue != null &&
					ValidValues.Keys.Contains(selectedValue)) {
					
					return ValidValues[selectedValue];
				}
				
				return null;
			}
			set {
				selectedValue = value;
			}
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
			reader.ReadStartElement("DropdownButton");
			SelectedValue = reader.ReadString();
			reader.ReadEndElement();
        }

        public void WriteXml(XmlWriter writer) {
            writer.WriteValue(this.ToString());
        }

        #endregion

		public override string ToString() {
			// Change to SelectedValue in the future. Have to check for backward compatibility
			return selectedValue;
		}
    }
}