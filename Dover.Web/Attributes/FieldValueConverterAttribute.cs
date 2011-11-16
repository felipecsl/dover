using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Com.Dover.Attributes {
	public class FieldValueConverterAttribute : Attribute {
		public Type ConverterType { get; set; }
	}
}