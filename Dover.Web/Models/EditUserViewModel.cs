using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Com.Dover.Profile;
using Com.Dover.Modules;
using System.Web.Mvc;

namespace Com.Dover.Web.Models {
    public class EditUserViewModel {
        public Guid UserId { get; set; }
		public string Email { get; set; }
		public bool Administrator { get; set; }
        public UserProfile UserProfile { get; set; }
        public IEnumerable<IModule> UserModules { get; set; }
        public IEnumerable<IModule> AllModules { get; set; }
    }
}
