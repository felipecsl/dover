using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Com.Dover.Modules;
using Com.Dover.Web.Models;
using System.Collections.Specialized;
using Com.Dover.Web.Models.Converters;
using System.Xml.Linq;

namespace Com.Dover.Web.Api {
	/// <summary>
	/// Dover API - Module Manipulation
	/// </summary>
	public class ModuleApi {
		private IModuleRepository ModRepository;

		public ModuleApi(IModuleRepository _repo) {
			ModRepository = _repo;
		}

		public ModuleApi() : this(new ModuleRepository()) {
		}

		/// <summary>
		/// Returns module records based on the provided module module
		/// and filtering parameters.
		/// </summary>
		/// <param name="module">The module to be queried</param>
		/// <param name="id">Optional row id for filtering</param>
		/// <returns>The matching rows in the specified module</returns>
		public DynamicModuleApiResult GetModuleData(IModule module, int? id, NameValueCollection _filters) {
			if (module == null) {
				return null;
			}

			var moduleData = new DynamicModuleApiResult() {
				ModuleName = module.ModuleName,
				ModuleType = module.ModuleType,
				ModuleId = module.Id
			};

			var rows = module.Rows;
			var filteredRows = (id != null) ? rows.Where(r => r.ID == id) : rows.AsEnumerable();

			// Search filter
			string reqFilter = _filters["_Filter"];

			if (!String.IsNullOrWhiteSpace(reqFilter)) {
				filteredRows = filteredRows.Where(r => r.Cells.Any(rf => rf.Data != null && rf.Data.IndexOf(reqFilter, StringComparison.CurrentCultureIgnoreCase) != -1));
			}

			// Random entry filter
			string randomEntry = _filters["_Random"];
			int randomTotal;
			if (!String.IsNullOrWhiteSpace(randomEntry) && Int32.TryParse(randomEntry, out randomTotal)) {
				filteredRows = filteredRows.OrderBy(r => Guid.NewGuid()).Take(randomTotal);
			}

			foreach (var key in _filters.AllKeys.Except(new string[] { "_Filter", "_Random" })) {
				var fieldName = key;
				var value = _filters[fieldName];
				filteredRows = filteredRows.Where(r => r.Cells.Any(rf => rf.Field.FieldName == fieldName && rf.Data != null && rf.Data.IndexOf(value, StringComparison.CurrentCultureIgnoreCase) != -1));
			}

			foreach (var row in filteredRows.OrderBy(r => r.SortIndex)) {
				DynamicModuleViewModel modelRow = new DynamicModuleViewModel() { ID = row.ID };

				foreach (Cell rowField in row.Cells) {
					var field = rowField.Field;
					var sFieldDataType = field.FieldDataType.Name;
					var fieldDataTpe = Type.GetType(sFieldDataType);

					var modelField = new DynamicModuleField() {
						DisplayName = field.DisplayName,
						PropertyName = field.FieldName,
						IsReadOnly = field.IsReadOnly,
						IsRequired = field.IsRequired,
						DataType = fieldDataTpe
					};

					var converter = FieldValueConversion.GetConverter(fieldDataTpe);
					modelField.Data = converter.Deserialize(
						rowField,
						new ConversionContext {
							Field = field,
							Module = module,
							Repository = ModRepository
						});

					modelRow.Fields.Add(modelField);
				}

				moduleData.Rows.Add(modelRow);
			}
			
			return moduleData;
		}

		public void SetModuleData(IModule _dbModule, int? id, DynamicModuleViewModel _moduleData, bool partialData = false) {
			if (_dbModule.ModuleType == (int)ModuleType.SingleEntry &&
				_dbModule.Rows.FirstOrDefault() == null) {

				// We have a single entry module and its row hasn't been created yet.
				// No big deal, we'll do it.
				CreateModule(_dbModule as Module, _moduleData);
			}

			var rowToEdit = (_dbModule.ModuleType == (int)ModuleType.Dynamic)
				? _dbModule.Rows.FirstOrDefault(row => row.ID == id)
				: _dbModule.Rows.FirstOrDefault();

			if (rowToEdit == null) {
				throw new ModuleRowNotFoundException("Registro não encontrado.");
			}

			if (!partialData) {
				// Update all the row fields, i.e. set null to missing form fields.
				foreach (Cell rowField in rowToEdit.Cells) {
					var formField = _moduleData.Fields.FirstOrDefault(rf => rf.PropertyName == rowField.Field.FieldName);
					var converter = FieldValueConversion.GetConverter(formField.DataType);

					rowField.Data = converter.Serialize(formField, new ConversionContext {
						Cell = rowField,
						Field = rowField.Field,
						Module = _dbModule,
						Repository = ModRepository
					});
				}
			}
			else {
				// Update only the row fields that were received as form fields. Do not touch others.
				foreach (var formField in _moduleData.Fields) {
					var rowField = rowToEdit.Cells.FirstOrDefault(rf => rf.Field.FieldName == formField.PropertyName);
					var converter = FieldValueConversion.GetConverter(formField.DataType);

					rowField.Data = converter.Serialize(formField, new ConversionContext {
						Cell = rowField,
						Field = rowField.Field,
						Module = _dbModule,
						Repository = ModRepository
					});
				}
			}

			ModRepository.Save();
		}

		public void SetPartialModuleData(IModule _dbModule, int? id, DynamicModuleViewModel _moduleData) {
			SetModuleData(_dbModule, id, _moduleData, true);
		}

		public void CreateModule(IModule _dbModule, DynamicModuleViewModel _moduleData) {
			var row = new Row() { Module = (Module)_dbModule };

			foreach (DynamicModuleField field in _moduleData.Fields) {
				var converter = FieldValueConversion.GetConverter(field.DataType);
				var rowField = new Cell {
					Field = _dbModule.Fields.FirstOrDefault(f => f.FieldName == field.PropertyName),
					Data = converter.Serialize(field, new ConversionContext { Cell = null })
				};

				row.Cells.Add(rowField);
			}

			try {
				ModRepository.AddModuleEntry(_dbModule.Id, row);
			}
			catch (Exception e) {
				throw new CreateModuleFailedException("Ocorreu um erro ao incluir o seu registro. Tente novamente mais tarde.", e);
			}
		}
	}
}