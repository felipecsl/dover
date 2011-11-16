using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Com.Dover.Controllers;
using System.Web.Routing;
using Com.Dover.Modules;
using Br.Com.Quavio.Tools.Web;

namespace Com.Dover.Attributes {
    /// <summary>
    /// Validation code for common dynamic module controller actions
    /// </summary>
    public class DynamicModuleActionAttribute : ActionFilterAttribute {

        public DynamicModuleActionAttribute() {
            CheckLogin = true;
        }
        
        public bool CheckLogin { get; set; }
        
        public override void OnActionExecuting(ActionExecutingContext filterContext) {
            var controller = filterContext.Controller as DoverController;
            var moduleId = filterContext.RouteData.Values["moduleid"] as string;
            int nModuleId;

            if(controller != null) {
                if(!String.IsNullOrWhiteSpace(moduleId)) {
                    if(Int32.TryParse(moduleId, out nModuleId)) {
                        var theModule = controller.ModRepository.GetModuleById(nModuleId,
							m => m.Account,
							m => m.Fields.Include<Field, FieldDataType>(f => f.FieldDataType), 
							m => m.Rows.Include<Row, Cell>(r => r.Cells));

                        var redirectResult = new RedirectToRouteResult(
                            new RouteValueDictionary(new {
                                Controller = "Home",
                                Action = "Index"
                            }));

                        if(theModule == null) {
                            controller.TempData["Message"] = "Módulo não encontrado.";
                            filterContext.Result = redirectResult;
                        }
						else if (!IsAuthorized(controller, theModule)) {
                            controller.TempData["Message"] = "Usuário não autorizado.";
                            filterContext.Result = redirectResult;
                        }

                        filterContext.ActionParameters["module"] = theModule;
                    }
                }
            }
            
            base.OnActionExecuting(filterContext);
        }

		private bool IsAuthorized(DoverController controller, IModule module) {
			if(!CheckLogin) {
				return true;
			}
			
			var user = controller.MembershipService.GetUser();

			if(user == null) {
				return false;
			}

			if (controller.MembershipService.IsUserInRole(user.UserName, "sysadmin")) {
				return true;
			}

			if (!module.Account.Users.IsLoaded) {
				module.Account.Users.Load();
			}

			return (module.Account.Users.FirstOrDefault(u => u.UserId == (Guid)user.ProviderUserKey) != null);
		}
    }
}