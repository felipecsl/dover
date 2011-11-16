using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Web.Security;
using Com.Dover.Web.Models;
using Com.Dover.Modules;
using Br.Com.Quavio.Tools.Web;
using Com.Dover.Web.Models.DataTypes;
using Com.Dover.Helpers;
using Com.Dover.Attributes;

namespace Com.Dover.Controllers {
	[Authorize(Roles = "administrators, sysadmin")]
    [HandleErrorWithELMAH]
    public class ModulesController : DoverController {
        public ModulesController() {
        }

		public ModulesController(IModuleRepository _repository, IMembershipService _membership)
			: base(_repository, _membership) {
        }


        [HttpPost]
        public ActionResult AddModules(FormCollection _vars) {
            return SaveModules(_vars, SaveModulesOperation.Add);
        }

        [HttpPost]
        public ActionResult RemoveModules(FormCollection _vars) {
            return SaveModules(_vars, SaveModulesOperation.Remove);
        }

		private SelectList GetDataTypeList() {
			var dataTypes = ModRepository.GetFieldDataTypes()
				.OrderBy(f => f.FriendlyName)
				.Select(f => new {
					Name = f.FriendlyName,
					Value = f.Name
				});

			return new SelectList(dataTypes, "Value", "Name", null);
		}

        private static SelectList GetUserList() {
            var itemList = new List<object>();

            foreach(MembershipUser user in Membership.GetAllUsers()) {
                itemList.Add(new {
                    Text = user.UserName,
                    Value = user.ProviderUserKey
                });
            }

            return new SelectList(itemList, "Value", "Text", null);
        }

		public ActionResult Edit(int id) {
			var modToEdit = ModRepository.GetModuleById(id,
				m => m.Fields.Include<Field, FieldDataType>(f => f.FieldDataType),
				m => m.Fields.Include<Field, FieldMetadata>(f => f.Metadata));

			if (modToEdit == null) {
				TempData["Message"] = "Módulo não encontrado.";
				return RedirectToAction("Index", "Home");
			}

			ModulesViewModel model = new ModulesViewModel() {
				Id = modToEdit.Id,
				DisplayName = modToEdit.DisplayName,
				DataTypeList = GetDataTypeList(),
				Type = modToEdit.ModuleType
			};

			model.Fields.AddRange(
				modToEdit.Fields.ToList().Select(f => new ModuleField {
					ID = f.ID,
					IsRequired = f.IsRequired,
					IsReadOnly = f.IsReadOnly,
					DataType = f.FieldDataType.Name,
					FieldDisplayName = f.DisplayName,
					ShowInListMode = f.ShowInListMode,
					Metadata = f.Metadata.FirstOrDefault() != null ? f.Metadata.FirstOrDefault().Value : null // *1
				}));

			// *1 TODO: Despite the field-metadata relationship is 1-N, we're passing only the first entry to the view for now.
			// This will probably change in the future when we have multiple metadata entries for each field.
			return View(model);
		}

		[HttpPost]
		public ActionResult Edit(ModulesViewModel _model) {
			if (!ModelState.IsValid) {
				_model.DataTypeList = GetDataTypeList();
				return View(_model);
			}

			var modToEdit = ModRepository.GetModuleById(_model.Id,
				m => m.Fields.Include<Field, FieldDataType>(f => f.FieldDataType),
				m => m.Fields.Include<Field, FieldMetadata>(f => f.Metadata),
				m => m.Rows.Include<Row, Cell>(r => r.Cells));

			if (modToEdit == null) {
				TempData["Message"] = "Módulo não encontrado.";
				_model.DataTypeList = GetDataTypeList();
				return View(_model);
			}

			modToEdit.DisplayName = _model.DisplayName;

			if (_model.Fields.Count(f => f.ShowInListMode) == 0) {
				TempData["Message"] = "Pelo menos um dos campos deve ser visível na listagem.";
				_model.DataTypeList = GetDataTypeList();
				return View(_model);
			}

			// handle modified and added fields
			foreach (ModuleField f in _model.Fields) {
				Field dbField = modToEdit.Fields.FirstOrDefault(fld => fld.ID == f.ID);

				bool bNeedToAdd = (dbField == null);

				if (dbField == null) {
					dbField = new Field();
				}

				if (!bNeedToAdd) {
					dbField.FieldDataTypeReference.Load();
					dbField.Metadata.Load();

					if (dbField.FieldDataType.Name != f.DataType) {
						// field type changed. set it to null on all matching rows
						modToEdit.Rows.ToList().ForEach(r => {
							r.Cells.Load();

							r.Cells
								.ToList()
								.ForEach(rf => {
									rf.FieldReference.Load();

									if (rf.Field.FieldName == dbField.FieldName) {
										rf.Data = null;
									}
								});
						});
					}
				}

				dbField.DisplayName = f.FieldDisplayName;
				dbField.FieldName = f.FieldDisplayName.EscapeName();
				dbField.FieldDataType = ModRepository.GetDataTypeByName(f.DataType);
				dbField.IsReadOnly = f.IsReadOnly;
				dbField.IsRequired = f.IsRequired;
				dbField.ShowInListMode = f.ShowInListMode;
				dbField.Module = modToEdit as Module;

				if (f.Metadata != null) {
					dbField.AddMetadata(f.Metadata);
				}

				if (bNeedToAdd) {
					ModRepository.AddField(dbField);

					modToEdit.Rows.ToList()
						.ForEach(row => {
							Cell rf = new Cell() { Field = dbField, Row = row };
							ModRepository.AddCell(rf);
						});
				}
			}

			ModRepository.Save();

			TempData["Message"] = "Módulo salvo com sucesso.";
			return RedirectToAction("List", "DynamicModule", new { moduleid = _model.Id, modulename = modToEdit.ModuleName });
		}

        public ActionResult Create() {
            ModulesViewModel model = new ModulesViewModel() {
                DataTypeList = GetDataTypeList()
            };

            return View(model);
        }

		public ActionResult FieldEditor(string fieldName) {
			return PartialView(fieldName);
		}

        [HttpPost]
        public ActionResult Create(ModulesViewModel _model) {
			if (!ModelState.IsValid) {
				_model.DataTypeList = GetDataTypeList();
				return View(_model);
			}

            if(_model.Fields.Count(f => f.ShowInListMode) == 0) {
                TempData["Message"] = "Pelo menos um dos campos deve ser visível na listagem.";
                _model.DataTypeList = GetDataTypeList();
                return View(_model);
            }

			// retrieve the current account name and associate with the newly created module
			var acctName = RouteData.Values["account"] as string;

			if (String.IsNullOrWhiteSpace(acctName)) {
				TempData["Message"] = "Conta não encontrada.";
				return View(_model);
			}

			Module newModule = new Module() {
                DisplayName = _model.DisplayName,
                ModuleName = _model.DisplayName.EscapeName(),
                User = ModRepository.GetUserByName(Membership.GetUser().UserName),	// associate the module with the current logged in user
				Account = ModRepository.GetAccountByName(acctName),
                ModuleType = _model.Type
            };

			_model.Fields.ToList().ForEach(f => {
				var newField = new Field {
					DisplayName = f.FieldDisplayName,
					FieldName = f.FieldDisplayName.EscapeName(),
					FieldDataType = ModRepository.GetDataTypeByName(f.DataType),
					IsReadOnly = f.IsReadOnly,
					IsRequired = f.IsRequired,
					ShowInListMode = f.ShowInListMode
				};

				if (f.Metadata != null) {
					newField.AddMetadata(f.Metadata);
				}
				newModule.Fields.Add(newField);
			});

            ModRepository.AddModule(newModule);

			TempData["Message"] = "Módulo salvo com sucesso.";
			return RedirectToAction("Index", "Home");
        }

		[HttpPost]
		public ActionResult Delete(int id) {
			
			var modToDelete = ModRepository.GetModuleById(id);

			if (modToDelete == null) {
				TempData["Message"] = "Módulo não encontrado.";
				return RedirectToAction("Index", "Home");
			}

			ModRepository.DeleteObject(modToDelete);
			ModRepository.Save();

			TempData["Message"] = "Módulo removido com sucesso.";
			return RedirectToAction("Index", "Home");
		}

		[HttpPost]
		public ActionResult DeleteField(int id) {
			var field = ModRepository.GetFieldById(id);

			if (field == null) {
				return Json(new { result = -1, msg = "Erro: Campo não encontrado." });
			}

			// check if the user is authorized to remove that field
			field.ModuleReference.Load();
			field.Module.AccountReference.Load();
			field.Module.Account.Users.Load();

			if (!field.Module.Account.Users.Any(u => u.UserName == Membership.GetUser().UserName)) {
				return Json(new { result = -1, msg = "Erro: O usuário não está autorizado a executar esta operação." });
			}

			ModRepository.DeleteObject(field);
			ModRepository.Save();

			return Json(new { result = 0, msg = "Campo removido com sucesso." });
		}

        private JsonResult SaveModules(
            FormCollection _vars,
            SaveModulesOperation _op) {
            if(String.IsNullOrEmpty(_vars["userid"])) {
				return Json(new { result = -1, msg = "Erro: Identificação do usuário inválida." });
            }

            Guid userID;
            try {
                userID = new Guid(_vars["userid"]);
            }
            catch(Exception e) {
                return Json(new { result = -1, msg = "Identificação do usuário em formato inválido.\n" + e.Message });
            }

            _vars.Remove("userid");

            Action<Guid, int> del;

            if(_op == SaveModulesOperation.Add) {
                del = ModRepository.AddUserModule;
            }
            else {
                del = ModRepository.RemoveUserModule;
            }

            foreach(string s in _vars) {
                try {
                    del(userID, Int32.Parse(_vars[s]));
                }
                catch(Exception e) {
                    return Json(new { result = -1, msg = "Falha ao adicionar o módulo selecionado.\n" + e.Message });
                }
            }

            return Json(new { result = 0, msg = "Sucesso." });
        }

        protected override ViewResult View(string viewName, string masterName, object model) {
            ModelMetadataProviders.Current = new DataAnnotationsModelMetadataProvider();

            return base.View(viewName, masterName, model);
        }

		public ActionResult GetAccountModules() {
			try {
				var modList = new List<object>();

				foreach (var m in ModRepository.GetAccountModules()) {
					modList.Add(new {
						moduleName = m.DisplayName,
						id = m.Id
					});
				}

				return Json(new {
					status = 1,
					modules = modList
				}, JsonRequestBehavior.AllowGet);
			}
			catch (Exception) {
				return Json(new {
					status = 0,
				}, JsonRequestBehavior.AllowGet);
			}
		}
    }

    public enum SaveModulesOperation {
        Add,
        Remove
    }
}