using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Com.Dover.Modules;

namespace Com.Dover.Web.Models.Converters {
	public interface IFieldValueConverter {
		Type DataType { get; set; }

		string Serialize(DynamicModuleField _obj, ConversionContext _context = null);
		object Deserialize(Cell _data, ConversionContext _context = null);
	}
}
