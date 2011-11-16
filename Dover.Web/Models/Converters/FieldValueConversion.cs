using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Br.Com.Quavio.Tools.Web;
using Com.Dover.Helpers;
using Com.Dover.Attributes;

namespace Com.Dover.Web.Models.Converters {
	public class FieldValueConversion {
		public static IFieldValueConverter GetConverter(Type t) {

			if (!DefaultFieldValueConverter.SupportsType(t.Name) &&
				t.GetInterface("IFieldDataType") == null ) {
				throw new ArgumentException("The provided type is not a valid field or does not implement the interface IFieldDataType");
			}

			var attribs = t.GetCustomAttributes(typeof(FieldValueConverterAttribute), false).AsEnumerable();
			var defConverter = new DefaultFieldValueConverter { DataType = t };

			if (attribs.IsNullOrEmpty()) {	// no custom converter was found. return the default converter
				return defConverter;
			}

			var attrib = attribs.FirstOrDefault() as FieldValueConverterAttribute;

			if (attrib == null) {
				return defConverter;
			}

			try {
				var converterInstance = Activator.CreateInstance(attrib.ConverterType) as IFieldValueConverter;

				if (converterInstance == null) {
					// the provided converter type does not implement the interface IFieldValueConverter
					return defConverter;
				}

				converterInstance.DataType = t;
				return converterInstance;
			}
			catch (Exception) {
				// an error has occurred instantiating the converter type
 				// cannot get the custom converter. return the default one
				return defConverter;
			}
		}
	}
}