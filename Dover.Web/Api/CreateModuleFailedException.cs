using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Dover.Web.Api {
	class CreateModuleFailedException : Exception {
		public CreateModuleFailedException() {
		}
		
		public CreateModuleFailedException(string msg, Exception inner) 
			: base(msg, inner) {
		}
	}
}
