using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Web.Mvc;
using Br.Com.Quavio.Tools.Web;
using Com.Dover.Modules;
using Com.Dover.Web.Models;
using Com.Dover.Web.Models.Converters;
using Com.Dover.Web.Api;
using Com.Dover.Helpers;
using Com.Dover.Web.Models.DataTypes;
using Com.Dover.Attributes;

namespace Com.Dover.Controllers {
	/// <summary>
	/// Dover API controller endpoint
	/// </summary>
	[ValidateInput(false), HandleErrorWithELMAH]
	public class ApiController : DoverController {
		public ApiController() {
        }

		public ApiController(IModuleRepository _repository, IMembershipService _membership) 
		 : base(_repository, _membership) {
        }

		public ActionResult Index() {
			return Content(String.Empty);
		}

		/// <summary>
		/// Dover action for returning module records based on the 
		/// provided module name, id and filter parameters.
		/// </summary>
		/// <param name="moduleid">The module id to be queried</param>
		/// <param name="modulename">The module name to filter</param>
		/// <param name="id">Optional row id for filtering</param>
		/// <returns>The matching rows in the specified module</returns>
		[ApiEndPoint(Order = 1), CompressFilter(Order = 2), CacheFilter(Duration = 60, Order = 3)]
		public ActionResult module(int moduleid, int? id) {
			var doverApi = new ModuleApi(ModRepository);

			var module = ModRepository.GetModuleById(moduleid,
				m => m.Fields.Include<Field, FieldDataType>(f => f.FieldDataType),
				m => m.Rows.Include<Row, Cell>(r => r.Cells));

			if (module == null) {
				return View();
			}

			var data = doverApi.GetModuleData(module, id, Request.QueryString);

			ModRepository.IncrementModuleRequestCount(module.Id);

			return View(data);
		}

		/// <summary>
		/// Dover action for returning a list of modules records based on the 
		/// provided module ids.
		/// </summary>
		/// <param name="moduleid">The list of module ids to be queried</param>
		/// <returns>All the records specified in the provided module ids</returns>
		[ApiEndPoint(Order = 1), CompressFilter(Order = 2), CacheFilter(Duration = 60, Order = 3)]
		public ActionResult modules(string moduleid) {
			var dataList = new DynamicModuleApiResultList();

			if (!String.IsNullOrWhiteSpace(moduleid) &&
				Request.QueryString.Count == 0) {

				var doverApi = new ModuleApi(ModRepository);

				string[] ids = moduleid.Split(",".ToCharArray());

				foreach (var id in ids) {
					int nid;
					if (!Int32.TryParse(id.Trim(), out nid)) {
						continue;
					}

					var module = ModRepository.GetModuleById(nid,
						m => m.Fields.Include<Field, FieldDataType>(f => f.FieldDataType),
						m => m.Rows.Include<Row, Cell>(r => r.Cells));

					if (module == null) {
						continue;
					}

					dataList.Add(doverApi.GetModuleData(module, null, Request.QueryString));

					ModRepository.IncrementModuleRequestCount(module.Id);
				}
			}

			return View(dataList);
		}

		[AcceptVerbs(HttpVerbs.Put), ValidateInput(false)]
		public ActionResult module(int moduleid, DynamicModuleViewModel _entryToCreate) {
			var doverApi = new ModuleApi(ModRepository);
			var module = ModRepository.GetModuleById(moduleid, m => m.Fields);
			var result = String.Empty;
			var filteredList = new DynamicModuleFieldList();
			
			filteredList.AddRange(_entryToCreate.Fields.Where(f => CheckFieldExistency(f)));
			_entryToCreate.Fields = filteredList;

			try {
				doverApi.CreateModule(module, _entryToCreate);
				result = "<result>Success</result>";
			}
			catch (CreateModuleFailedException) {
				result = "<result>Failure</result>";
			}

			return Content(result, "text/xml", Encoding.UTF8);
		}

		/// <summary>
		/// Dover action for updating a module data based on the provided
		/// module name, id and new values.
		/// </summary>
		/// <param name="moduleid">The id of the module to be update</param>
		/// <param name="modulename">The name of the module to be updated</param>
		/// <param name="id">The id of the record to be updated.</param>
		/// <param name="_entryToEdit">The new values.</param>
		/// <returns>Xml/Json with either success or failure.</returns>
		[HttpPost, ValidateInput(false)]
		public ActionResult module(int moduleid, int? id, DynamicModuleViewModel _entryToEdit) {
			var result = String.Empty;

			try {
				var doverApi = new ModuleApi(ModRepository);
				var module = ModRepository.GetModuleById(moduleid, m => m.Rows.Include<Row, Cell>(r => r.Cells), m => m.Fields);
				var filteredList = new DynamicModuleFieldList();

				filteredList.AddRange(_entryToEdit.Fields.Where(f => CheckFieldExistency(f)));
				_entryToEdit.Fields = filteredList;
				
				doverApi.SetModuleData(module, id, _entryToEdit, true);
				result = "<result>Success</result>";
			}
			catch (Exception e) {
				result = "<result>Failure</result>";
			}

			return Content(result, "text/xml", Encoding.UTF8);
		}

		/// <summary>
		/// Filters a field that was not found in the Fom keys collection
		///	This allows us to update/add only the fields that were sent via API, i.e., 
		///	the developer does not need to always send all the row fields.
		/// </summary>
		/// <param name="_model">A DynamicModuleViewModel bound to an Action method</param>
		/// <returns>A DynamicModuleFieldList with only the fields found in the request vars</returns>
		private bool CheckFieldExistency(DynamicModuleField _field) {

			string[] array = new string[1000];

			var a = Request;
			var b = a.Form;
			array = b.AllKeys;

			return (_field.Data != null) ||
				array.Any(k => CheckFormKey(k, _field.PropertyName));
		}

		private bool CheckFormKey(string key, string propName) {
			if (String.IsNullOrWhiteSpace(key)) {
				return false;
			}

			var fixedKey = key;
			if (fixedKey.IndexOf('.') > 0) {
				fixedKey = fixedKey.Substring(0, fixedKey.IndexOf('.'));
			}

			return fixedKey == propName;
		}
	}
}