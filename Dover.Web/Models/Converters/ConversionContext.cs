using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Com.Dover.Modules;

namespace Com.Dover.Web.Models.Converters {
	public class ConversionContext {
		public ConversionContext() {
		}

		public Field Field { get; set; }
		public Cell Cell { get; set; }
		public IModule Module { get; set; }
		public IModuleRepository Repository { get; set; }
	}
}