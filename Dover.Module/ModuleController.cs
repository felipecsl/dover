using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Data.SqlClient;
using Com.Dover.Profile;
using System.Web.Security;
using Br.Com.Quavio.Tools.Web.Mvc;
using System.Data.EntityClient;

namespace Com.Dover.Modules {
    public abstract class ModuleController : DoverController {
        public ModuleController() {
        }

		public ModuleController(IModuleRepository _repo, IMembershipService _membership)
			: base(_repo, _membership) {
		}
    }
}
