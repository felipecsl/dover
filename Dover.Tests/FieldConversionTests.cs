using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Com.Dover.Web.Models.Converters;
using Com.Dover.Web.Models.DataTypes;

namespace Com.Dover.Tests {
	[TestClass]
	public class FieldConversionTests {
		[TestMethod]
		public void TestGetDefaultConverter() {
			var converter = FieldValueConversion.GetConverter(typeof(HtmlText));

			Assert.IsNotNull(converter);
			Assert.IsInstanceOfType(converter, typeof(DefaultFieldValueConverter));
		}

		[TestMethod]
		public void TestGetCustomConverter() {
			var converter = FieldValueConversion.GetConverter(typeof(ModuleReference));

			Assert.IsNotNull(converter);
			Assert.IsInstanceOfType(converter, typeof(ModuleReferenceConverter));
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void TestInvalidDataType() {
			FieldValueConversion.GetConverter(typeof(UInt32));
		}
	}
}
