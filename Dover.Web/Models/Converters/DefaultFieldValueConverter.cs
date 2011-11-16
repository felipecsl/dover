using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Br.Com.Quavio.Tools.Web;
using Com.Dover.Modules;
using System.Text;
using System.Xml;
using System.IO;
using System.Xml.Serialization;

namespace Com.Dover.Web.Models.Converters {
	public class DefaultFieldValueConverter : IFieldValueConverter {
		public static FieldSerializationMethod SerializationMethod = FieldSerializationMethod.Xml;
		public Type DataType { get; set; }
		private static XmlWriterSettings xmlSettings = new XmlWriterSettings { OmitXmlDeclaration = true };
		
		public DefaultFieldValueConverter() {
		}

		#region IFieldValueConverter Members

		public virtual string Serialize(DynamicModuleField _obj, ConversionContext _context = null) {
			if (_obj == null || _obj.Data == null) {
				return null;
			}

			switch (DataType.Name) {
				case "String":
				case "Int32":
				case "DateTime":
				case "Boolean":
					return _obj.Data.ToString();
				default:
					return GenerateXml(_obj.Data);
			}
		}

		public virtual object Deserialize(Cell _data, ConversionContext _context = null) {
			if (_data == null || 
				String.IsNullOrWhiteSpace(_data.Data)) {
				return null;
			}

			string strData = _data.Data;

			switch (DataType.Name) {
				case "String":
					return strData;
				case "Int32":
					int result;
					if (Int32.TryParse(strData, out result)) {
						return result;
					}
					return null;
				case "DateTime":
					DateTime dt;
					if (DateTime.TryParse(strData, out dt)) {
						return dt;
					}
					return null;
				case "Boolean":
					bool b;
					if (Boolean.TryParse(strData, out b)) {
						return b;
					}
					return null;
				default:
					return ParseXml(strData, DataType);
			}
		}

		public static string GenerateXml(object data) {
			using (MemoryStream stream = new MemoryStream()) {
				using (var xmlWriter = XmlTextWriter.Create(stream, xmlSettings)) {
					new XmlSerializer(data.GetType()).Serialize(xmlWriter, data);
				}
				using (var reader = new StreamReader(stream, Encoding.UTF8)) {
					stream.Position = 0;	// Need to reset the stream position, otherwise StreamReader cannot read it :(
					string result = reader.ReadToEnd();
					return result;
				}
			}
		}

		public static object ParseXml(string strData, Type dataType) {
			using(var stream = new MemoryStream(Encoding.UTF8.GetBytes(strData))) {
				using (var xmlReader = XmlTextReader.Create(stream)) {
					return new XmlSerializer(dataType).Deserialize(xmlReader);
				}
			}
		}

		public static bool SupportsType(string _typeName) {
			return (_typeName == "String" ||
				_typeName == "DateTime" ||
				_typeName == "Boolean" ||
				_typeName == "Int32");
		}

		#endregion
	}

	public enum FieldSerializationMethod {
		Api,
		Xml
	}
}