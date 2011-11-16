using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;
using Com.Dover.Profile;
using System.Data.EntityClient;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace Com.Dover.Modules {
    public abstract class StaticModuleController : ModuleController {
        public StaticModuleController() {
        }

        public abstract int Id { get; set; }
        public abstract string DisplayName { get; set; }
        public abstract string ControllerName { get; set; }

        public virtual ActionResult MenuButton() {
            ViewData["ModuleName"] = ControllerName;
            ViewData["DisplayName"] = DisplayName;
            ViewData["SubMenu"] = true;

            return PartialView();
        }

        protected override ViewResult View(string viewName, string masterName, object model) {
            ModelMetadataProviders.Current = new DataAnnotationsModelMetadataProvider();

            return base.View(viewName, masterName, model);
        }
    }
}