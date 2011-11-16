using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;
using Com.Dover.Web.Models.DataTypes;
using System.Web.Script.Serialization;
using System.Xml.Schema;
using System.Xml;

namespace Com.Dover.Web.Models
{
	[XmlInclude(typeof(DbImage))]
	[XmlInclude(typeof(ImageList))]
	[XmlInclude(typeof(HtmlText))]
	public class DynamicModuleField : IXmlSerializable
	{
		// Field data type
		[ScriptIgnore]
		public Type DataType { get; set; }

		// ModelMetadata values
		public string DisplayName { get; set; }

		// Field data object
		public object Data { get; set; }
		
		[ScriptIgnore]
		public string PropertyName { get; set; }
		
		[ScriptIgnore]
		public bool IsRequired { get; set; }
		
		[ScriptIgnore]
		public bool IsReadOnly { get; set; }

		#region IXmlSerializable Members

		public XmlSchema GetSchema()
		{
			return null;
		}

		public void ReadXml(XmlReader reader)
		{
			throw new NotImplementedException();
		}

		public void WriteXml(XmlWriter writer)
		{
			if (Data is IFieldDataType)
			{
				IFieldDataType field = Data as IFieldDataType;
				field.WriteXml(writer);
			}
			else if (Data != null)
			{
				writer.WriteValue(Data);
			}
		}

		#endregion
	}
}