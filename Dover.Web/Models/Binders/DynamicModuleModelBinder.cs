using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Text;
using System.Globalization;
using Com.Dover.Modules;
using Br.Com.Quavio.Tools.Web;
using System.Web.Security;
using Com.Dover.Controllers;

namespace Com.Dover.Web.Models.Binders {
    public class DynamicModuleModelBinder : IModelBinder {
		private IModuleRepository ModRepository { get; set; }

		public DynamicModuleModelBinder(IModuleRepository _repository) {
			ModRepository = _repository;
        }

		public DynamicModuleModelBinder() 
			: this(new ModuleRepository()) {
		}
		
		#region IModelBinder Members

		public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext) {
			var routeValues = controllerContext.RouteData.Values;
			var id = -1;
			var moduleId = -1;

			var account = routeValues["account"] as string;
			var moduleName = routeValues["modulename"] as string;

			Int32.TryParse(routeValues["moduleid"] as string, out moduleId);
			Int32.TryParse(routeValues["id"] as string, out id);

			var module = ModRepository.GetModuleById(moduleId,
				m => m.Account,
				m => m.Fields.Include<Field, FieldDataType>(f => f.FieldDataType), 
				m => m.Rows.Include<Row, Cell>(r => r.Cells));

			if (module == null) {
				bindingContext.ModelState.AddModelError("_FORM", "Módulo não encontrado");
				return null;
			}
			
			if (module.Account == null ||
				module.Account.SubdomainName != account) {
				bindingContext.ModelState.AddModelError("_FORM", "Módulo não encontrado");
				return null;
			}

			// if we have a module name, use it for filtering
			if (!String.IsNullOrWhiteSpace(moduleName) &&
				module.ModuleName != moduleName) {
				bindingContext.ModelState.AddModelError("_FORM", "Módulo não encontrado");
				return null;
			}

			var theModel = new DynamicModuleViewModel() {
				ID = id,
				ModuleName = module.ModuleName,
				DisplayName = module.DisplayName
			};

			// Switch back to the original data annotations model metadata implementation
			ModelMetadataProviders.Current = new DataAnnotationsModelMetadataProvider();

			foreach (Field field in module.Fields) {
				field.FieldDataTypeReference.Load();
				string sFieldDataType = field.FieldDataType.Name;
				Type fieldDataTpe = Type.GetType(sFieldDataType);

				var modelField = new DynamicModuleField() {
					DisplayName = field.DisplayName,
					PropertyName = field.FieldName,
					IsReadOnly = field.IsReadOnly,
					IsRequired = field.IsRequired,
					DataType = fieldDataTpe
				};

				IModelBinder fieldBinder = ModelBinders.Binders.GetBinder(fieldDataTpe);

				var fieldBindingContext = new ModelBindingContext {
					ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(null, fieldDataTpe),
					ModelName = modelField.PropertyName,
					ModelState = bindingContext.ModelState,
					ValueProvider = bindingContext.ValueProvider,
					PropertyFilter = bindingContext.PropertyFilter
				};

				modelField.Data = fieldBinder.BindModel(controllerContext, fieldBindingContext);

				if (modelField.IsRequired &&
					(modelField.Data == null ||
					String.IsNullOrEmpty(modelField.Data.ToString()))) {
					bindingContext.ModelState.AddModelError(modelField.PropertyName, "O campo " + modelField.DisplayName + " é obrigatório");
				}

				theModel.Fields.Add(modelField);
			}

			return theModel;
		}

        #endregion
    }
}