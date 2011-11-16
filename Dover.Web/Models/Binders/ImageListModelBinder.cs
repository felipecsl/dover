using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Com.Dover.Web.Models.DataTypes;

namespace Com.Dover.Web.Models.Binders
{
	public class ImageListModelBinder : IModelBinder
	{
		#region IModelBinder Members

		public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
		{
			DefaultModelBinder binder = new DefaultModelBinder();	// use the default model binder to do the dirty work

			ModelBindingContext listBindingContext = new ModelBindingContext
			{
				ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => bindingContext.Model, bindingContext.ModelType),
				ModelName = bindingContext.ModelName,
				ModelState = bindingContext.ModelState,
				ValueProvider = bindingContext.ValueProvider,
				PropertyFilter = bindingContext.PropertyFilter
			};


			ImageList list = binder.BindModel(controllerContext, listBindingContext) as ImageList;

			return list;
		}

		#endregion
	}
}