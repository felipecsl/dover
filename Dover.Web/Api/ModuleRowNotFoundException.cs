using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Dover.Web.Api {
	public class ModuleRowNotFoundException : Exception {
		public ModuleRowNotFoundException() {
		}

		public ModuleRowNotFoundException(string message) 
			: this(message, null) {
		}

		public ModuleRowNotFoundException(string message, Exception inner) 
			: base(message, inner) {
		}
	}
}
