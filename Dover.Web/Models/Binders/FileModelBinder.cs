using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Com.Dover.Web.Models.DataTypes;
using Br.Com.Quavio.Tools.Web;
using Com.Dover.Helpers;
using System.Web.Security;
using Com.Dover.Profile;

namespace Com.Dover.Web.Models.Binders {
    public class FileModelBinder : IModelBinder {
        #region IModelBinder Members

        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext) {
            // a new file is being uploaded
            if (IsUploadingNewFile(controllerContext, bindingContext)) {
                var context = controllerContext.RequestContext.HttpContext;
				var user = Membership.GetUser() as UACUser;

				if (user == null) {
					throw new UnauthorizedAccessException("The user must be logged in to perform this action.");
				}

                var fInfo = user.SaveFile(context.Request.Files[bindingContext.ModelName]);

				var file = new File {
					FilePath = fInfo.FullRelativePath
				};

				if (!String.IsNullOrWhiteSpace(file.FilePath)) {
					var currRequest = controllerContext.RequestContext.HttpContext.Request;
					file.FilePath = currRequest.Url.Scheme + "://" + currRequest.Url.Host + UrlHelper.GenerateContentUrl(file.FilePath, controllerContext.RequestContext.HttpContext);
				}

                return file;
            }
            // just keep the original file path
            else if (IsKeepingOldFile(controllerContext, bindingContext)) {
                var originalFile = bindingContext.ValueProvider.GetValue(bindingContext.ModelName + ".FilePath");

                return new File() {
                    FilePath = originalFile.AttemptedValue
                };
            }
            
            return null;
        }

        #endregion

        private bool IsUploadingNewFile(ControllerContext controllerContext, ModelBindingContext bindingContext) {
            var valueResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            if (valueResult == null || String.IsNullOrWhiteSpace(valueResult.AttemptedValue)) {
                return false;
            }

            var context = controllerContext.RequestContext.HttpContext;
            var postedFile = context.Request.Files[bindingContext.ModelName];

            if (postedFile.ContentLength == 0 ||
                String.IsNullOrWhiteSpace(postedFile.FileName)) {
                return false;
            }

            return true;
        }

        private bool IsKeepingOldFile(ControllerContext controllerContext, ModelBindingContext bindingContext) {
            var originalFile = bindingContext.ValueProvider.GetValue(bindingContext.ModelName + ".FilePath");

            return originalFile != null && !String.IsNullOrWhiteSpace(originalFile.AttemptedValue);
        }
    }
}