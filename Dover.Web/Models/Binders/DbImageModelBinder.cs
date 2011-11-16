using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using Com.Dover.Web.Models.DataTypes;
using System.Web.Security;
using Com.Dover.Profile;

namespace Com.Dover.Web.Models.Binders {
    public class DbImageModelBinder : IModelBinder {
        #region IModelBinder Members

        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext) {
            
			var valueResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName + ".ImagePath");
            DbImage img = null;

			if (valueResult != null && !String.IsNullOrWhiteSpace(valueResult.AttemptedValue)) {
				img = new DbImage { ImagePath = valueResult.AttemptedValue };

				if (!String.IsNullOrWhiteSpace(img.ImagePath)) {
					if (img.ImagePath.Contains("?ts")) {
						img.ImagePath = img.ImagePath.Substring(0, img.ImagePath.LastIndexOf("?ts"));
					}
					if (img.ImagePath.StartsWith("/")) {
						var currRequest = controllerContext.RequestContext.HttpContext.Request;
						img.ImagePath = currRequest.Url.Scheme + "://" + currRequest.Url.Host + img.ImagePath;
					}
				}
			}
			else {
				// look for an uploaded image in the Form collection
				valueResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

				if (valueResult != null && valueResult.RawValue != null && valueResult.RawValue is HttpPostedFileBase[]) {
					var files = valueResult.RawValue as HttpPostedFileBase[];

					if (files.Length > 0) {
						var image = files[0];

						if (image != null &&
							image.ContentLength > 0) {
							UACUser currUser = null;

							if (controllerContext.HttpContext.User.Identity.IsAuthenticated) {
								currUser = Membership.GetUser() as UACUser;
							}
							else {
								var userName = bindingContext.ValueProvider.GetValue("doverUsername");
								currUser = Membership.GetUser(userName.AttemptedValue) as UACUser;
							}

							if (currUser != null) {
								var imageInfo = currUser.SaveImage(image);
								img = new DbImage { ImagePath = imageInfo.FullRelativePath };
							}
						}
					}
				}
			}

            return img;
        }

        #endregion
    }
}