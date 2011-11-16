using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Data.EntityClient;
using System.Collections.Generic;
using System.Web.Security;
using System.ComponentModel;
using System.Globalization;
using System.ServiceModel.Syndication;
using Microsoft.Web.Mvc;
using Br.Com.Quavio.Tools.Web;
using Com.Dover.Web.Models.Converters;
using Com.Dover.Helpers;
using Com.Dover.Web.Api;
using Com.Dover.Modules;
using Com.Dover.Profile;
using Com.Dover.Web.Models;
using Com.Dover.Web.Models.DataTypes;
using Com.Dover.Web.Models.MetadataProviders;
using Com.Dover.Attributes;

namespace Com.Dover.Controllers {
    [ValidateInput(false), HandleErrorWithELMAH]
    public class DynamicModuleController : ModuleController {
        
		public DynamicModuleController() {
        }

        public DynamicModuleController(
            IModuleRepository _repository,
			IMembershipService _membership)
			: base(_repository, _membership) {
        }

		/// <summary>
		/// Returns module records based on the provided module name, id and 
		/// filter parameters.
		/// </summary>
		/// <param name="moduleid">The module id to be queried</param>
		/// <param name="modulename">The module name to filter</param>
		/// <param name="id">Optional row id for filtering</param>
		/// <returns>The matching rows in the specified module</returns>
        [ApiEndPoint]
        public ActionResult Query(int moduleid, string modulename, int? id) {
			var doverApi = new ModuleApi(ModRepository);

			var module = ModRepository.GetModuleById(moduleid,
				m => m.Fields.Include<Field, FieldDataType>(f => f.FieldDataType),
				m => m.Rows.Include<Row, Cell>(r => r.Cells));

			var data = doverApi.GetModuleData(module, id, Request.QueryString);

			ModRepository.IncrementModuleRequestCount(module.Id);

			return View(data);
        }

		[HttpPost]
		public ActionResult PersistRowOrder(FormCollection form) {

			int i = 0;
			foreach (string s in form["ids[]"].Split(",".ToCharArray())) {
				int id;
				if (Int32.TryParse(s, out id)) {
					var row = ModRepository.GetRowById(id);
					row.SortIndex = i++;
				}
			}

			ModRepository.Save();

			return Json(new { });
		}

        [Authorize, DynamicModuleAction]
        public ActionResult List(object module) {
            var modToList = module as Module;

            ViewData["ModuleName"] = modToList.ModuleName;
            ViewData["DisplayName"] = modToList.DisplayName;
            ViewData["Fields"] = modToList.Fields.Where(f => f.ShowInListMode).Select(f => f.DisplayName);

            List<DynamicModuleViewModel> viewModel = new List<DynamicModuleViewModel>();

            foreach(Row _row in modToList.Rows.OrderBy(r => r.SortIndex)) {
                DynamicModuleViewModel modelRow = new DynamicModuleViewModel() { ID = _row.ID };

				foreach (Field field in modToList.Fields.Where(f => f.ShowInListMode)) {
					var rowField = field.Cells.FirstOrDefault(rf => rf.Row.ID == _row.ID);
					var fieldDataTpe = Type.GetType(field.FieldDataType.Name);
					var converter = FieldValueConversion.GetConverter(fieldDataTpe);

					var modelField = new DynamicModuleField() {
						DisplayName = field.DisplayName,
						PropertyName = field.FieldName,
						IsReadOnly = field.IsReadOnly,
						IsRequired = field.IsRequired,
						DataType = fieldDataTpe
					};

					modelField.Data = converter.Deserialize(
						rowField,
						new ConversionContext {
							Field = field,
							Module = modToList,
							Repository = ModRepository
						});

                    modelRow.Fields.Add(modelField);
                }

                viewModel.Add(modelRow);
            }

            return View(viewModel);
        }

        [Authorize, DynamicModuleAction]
        public ActionResult Create(object module) {
			return ViewModule(module as Module, null, true);
        }

		[Authorize, DynamicModuleAction]
		public ActionResult Edit(object module, int? id) {
			return ViewModule(module as Module, id);
		}

		private ActionResult ViewModule(Module _module, int? id, bool creating = false) {
			if (id != null && 
				_module.Rows.FirstOrDefault(row => row.ID == id) == null) {
				TempData["Message"] = "Registro não encontrado.";
				return this.RedirectToAction("List");
			}
			
			DynamicModuleViewModel modelRow = new DynamicModuleViewModel() {
				ID = id ?? -1,
				ModuleName = _module.ModuleName,
				DisplayName = _module.DisplayName
			};

			foreach (Field field in _module.Fields) {
				Cell rowField = null;

				if (!creating) {
					rowField = (id != null)
						? field.Cells.FirstOrDefault(rf => rf.Row.ID == id)
						: field.Cells.FirstOrDefault();
				}

				var fieldDataTpe = Type.GetType(field.FieldDataType.Name);
				var converter = FieldValueConversion.GetConverter(fieldDataTpe);
				var modelField = new DynamicModuleField() {
					DisplayName = field.DisplayName,
					PropertyName = field.FieldName,
					IsReadOnly = field.IsReadOnly,
					IsRequired = field.IsRequired,
					DataType = fieldDataTpe
				};

				modelField.Data = converter.Deserialize(
					rowField,
					new ConversionContext {
						Field = field,
						Module = _module,
						Repository = ModRepository
					});

				modelRow.Fields.Add(modelField);
			}

			return View(modelRow);
		}

		[HttpPost, ValidateInput(false), Authorize, DynamicModuleAction]
        public ActionResult Create(object module, DynamicModuleViewModel _entryToCreate) {
            if(!ModelState.IsValid) {
                return View(_entryToCreate);
            }

            var newModule = module as Module;

			try {
				var moduleApi = new ModuleApi(ModRepository);
				moduleApi.CreateModule(newModule, _entryToCreate);
			}
			catch (CreateModuleFailedException e) {
				TempData["Message"] = e.Message;
				return View(_entryToCreate);
			}

            TempData["Message"] = "Registro criado com sucesso.";

            return (newModule.ModuleType == (int)ModuleType.Dynamic)
                ? RedirectToAction("List") 
                : RedirectToAction("Index", "Home");
        }

        [HttpPost, ValidateInput(false), Authorize, DynamicModuleAction]
        public ActionResult Edit(object module, int? id, DynamicModuleViewModel _entryToEdit) {
            if(!ModelState.IsValid) {
                return View(_entryToEdit);
            }

            var moduleToEdit = module as Module;

			try {
				var moduleApi = new ModuleApi(ModRepository);
				moduleApi.SetModuleData(moduleToEdit, id, _entryToEdit);
			}
			catch (ModuleRowNotFoundException e) {
				TempData["Message"] = e.Message;
				return this.RedirectToAction("List");
			}

			TempData["Message"] = "Seus dados foram salvos.";

            return (moduleToEdit.ModuleType == (int)ModuleType.Dynamic)
                ? RedirectToAction("List")
                : RedirectToAction("Index", "Home");
        }

        protected override ViewResult View(string viewName, string masterName, object model) {
            ModelMetadataProviders.Current = new DynamicModuleFieldMetadataProvider();

            return base.View(viewName, masterName, model);
        }

		[Authorize, DynamicModuleAction]
		public ActionResult Delete(int id) {
			Row row = ModRepository.GetRowById(id);

			if (row == null) {
				TempData["Message"] = "Registro não encontrado.";

				return RedirectToAction("List");
			}

			ModRepository.DeleteObject(row);

			ModRepository.Save();

			TempData["Message"] = "Registro removido com sucesso.";
			return RedirectToAction("List");
		}
    }
}