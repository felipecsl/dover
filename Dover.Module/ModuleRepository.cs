using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Br.Com.Quavio.Tools;
using Br.Com.Quavio.Tools.Web;
using System.Web.Security;
using Com.Dover.Modules;
using Com.Dover.Profile;
using System.Data.SqlClient;
using System.Data.EntityClient;
using System.IO;
using System.Data;
using System.Linq.Expressions;
using System.Data.Objects;

namespace Com.Dover.Modules {
    public class ModuleRepository : IModuleRepository, IDisposable {

		public const string ModuleReferenceMetadataKey = "ModuleReferenceId";
		public const string DropdownButtonMetadataKey = "DropdownButtonId";
		public const string CheckBoxListMetadataKey = "CheckBoxListId";

		private DoverEntities db;

		public ModuleRepository(EntityConnection _connection) {
			db = (_connection != null)
				? new DoverEntities(_connection)
				: new DoverEntities();
		}

		public ModuleRepository() : this(null) {
		}

        public IModule GetStaticModuleByName(string _sName) {
            if(String.IsNullOrEmpty(_sName)) {
                throw new ArgumentNullException("Argumento Nome não deve ser nulo");
            }

            foreach(Type t in WebTools.TypesDerivingFrom(typeof(StaticModuleController))) {
                var ctrl = Activator.CreateInstance(t) as StaticModuleController;

                if(ctrl.ControllerName == _sName) {
                    return new Module {
                        DisplayName = ctrl.DisplayName,
                        ModuleName = ctrl.ControllerName,
                        ModuleType = (int)ModuleType.Static,
                        Id = ctrl.Id
                    };
                }
            }

            return null;
        }

        public IModule GetStaticModuleById(int _id) {
            foreach(Type t in WebTools.TypesDerivingFrom(typeof(StaticModuleController))) {
                if(!t.IsRealClass()) {
                    continue;
                }

                var ctrl = Activator.CreateInstance(t) as StaticModuleController;

                if(ctrl.Id == _id) {
                    return new Module {
                        DisplayName = ctrl.DisplayName,
                        ModuleName = ctrl.ControllerName,
                        ModuleType = (int)ModuleType.Static,
                        Id = ctrl.Id
                    };
                }
            }

            return null;
        }

        public IEnumerable<IModule> GetAllStaticModules() {
            var moduleList = new List<IModule>();

            foreach(Type t in WebTools.TypesDerivingFrom(typeof(StaticModuleController))) {
                if(!t.IsRealClass()) {
                    continue;
                }

                var ctrl = Activator.CreateInstance(t) as StaticModuleController;

                moduleList.Add(new Module {
                    DisplayName = ctrl.DisplayName,
                    ModuleName = ctrl.ControllerName,
                    ModuleType = (int)ModuleType.Static,
                    Id = ctrl.Id
                });
            }

            return moduleList;
        }

        /// <summary>
        /// Returns the first static or dynamic module found matching the provided name
        /// </summary>
        public IModule GetModuleByName(string _sModuleName) {
            IModule staticFound = GetStaticModuleByName(_sModuleName);

            if(staticFound != null) {
                return staticFound;
            }

            return db.Module
                .Include(m => m.Rows)
                .Include(m => m.Fields)
                .Include(m => m.Rows.First().Cells)
                .FirstOrDefault(m => m.ModuleName == _sModuleName);
        }

        /// <summary>
        /// Returns the first static or dynamic module found matching the provided uuid
        /// </summary>
		public IModule GetModuleById(int _id, params Expression<Func<Module, object>>[] subSelectors) {
			var query = db.Module as ObjectQuery<Module>;

			foreach (var selector in subSelectors) {
				query = query.Include(selector);
			}

			var module = query.FirstOrDefault(m => m.Id == _id);

			if (module != null) {
				return module;
			}

			return GetStaticModuleById(_id);
        }

        /// <summary>
        /// Returns a list with all modules currently enabled for the provided user id
        /// </summary>
        public IEnumerable<IModule> GetUserModules(Guid _userId) {
            try {
                return db.Module.Include(m => m.User).Where(m => m.User.UserId == _userId);
            }
            catch(Exception) {
                return null;
            }
        }

		public IEnumerable<IModule> GetAccountModules() {
			var subdomain = HttpContext.Current.Request.Url.GetSubDomain();

			if (!String.IsNullOrWhiteSpace(subdomain) && subdomain != "www") {
				return GetAccountModules(subdomain);
			}
			return new List<IModule>();
		}

		public IEnumerable<IModule> GetAccountModules(string accountName) {
			var acct = GetAccountByName(accountName);

			if (acct == null) {
				// account not found. return an empty module list?
				return new List<IModule>();
			}
			
			if (!acct.Modules.IsLoaded) {
				acct.Modules.Load();
			}

			return acct.Modules;
		}

		public IEnumerable<FieldDataType> GetFieldDataTypes() {
			return db.FieldDataType.ToList();
		}

        public void RemoveUserModule(Guid _userUuid, int _moduleId) {
			Module module = db.Module.Include(m => m.User).FirstOrDefault(m => m.User.UserId == _userUuid && m.Id == _moduleId);

            if(module == null) {
                throw new ArgumentException("Não foi encontrado um módulo com o uuid escolhido para este usuário");
            }

            db.DeleteObject(module);

            db.SaveChanges();
        }

        public void AddUserModule(
            Guid _userUuid,
            int _moduleId) {
            IModule m = GetModuleById(_moduleId, mod => mod.User);

            if(m == null) {
                throw new ArgumentException("Não foi encontrado um módulo com o uuid escolhido");
            }

            db.AddToModule(new Module() {
                User = db.User.SingleOrDefault(u => u.UserId == _userUuid),
                Id = _moduleId,
                ModuleName = m.ModuleName,
                DisplayName = m.DisplayName,
                ModuleType = (int)m.ModuleType
            });

            db.SaveChanges();
        }

        /// <summary>
        /// Returns a list with all modules currently enabled for the current logged in user
        /// </summary>
        public IEnumerable<IModule> GetUserModules() {
            MembershipUser currUser = Membership.GetUser();

            return (currUser != null)
                ? GetUserModules((Guid)currUser.ProviderUserKey)
                : new List<IModule>();
        }

        /// <summary>
        /// Adds a row (entry to the specified module)
        /// </summary>
        /// <param name="_entry"></param>
        public void AddModuleEntry(int _moduleId, Row _entry) {
            if(_entry == null) {
                throw new ArgumentNullException("_entry");
            }

            var theModule = db.Module.FirstOrDefault(m => m.Id == _moduleId);

            if(theModule == null) {
                throw new ArgumentException("Invalid module id");
            }

            db.Row.AddObject(_entry);
            db.SaveChanges();
        }

		/// <summary>
		/// Creates the provided module.
		/// </summary>
		/// <param name="_module">The module to be created</param>
		public void AddModule(Module _module) {
			if (_module == null) {
				throw new ArgumentNullException("_module");
			}

			db.AddToModule(_module);
			db.SaveChanges();
		}

		/// <summary>
		/// Adds the provided field.
		/// </summary>
		/// <param name="_field">The field to be created</param>
		public void AddField(Field _field) {
			db.AddToField(_field);
		}

		public void AddCell(Cell _Cell) {
			db.AddToCell(_Cell);
		}

		/// <summary>
		/// Gets the first row that matches the provided predicate or null if none found.
		/// </summary>
		/// <param name="predicate">The predicate.</param>
		public Row GetRowByPredicate(Expression<Func<Row, bool>> predicate) {
			return db.Row
				.Include(r => r.Cells)
				.FirstOrDefault(predicate);
		}

		/// <summary>
		/// Gets the first row field that matches the provided predicate or null if none found.
		/// </summary>
		/// <param name="predicate">The predicate.</param>
		public Cell GetCellByPredicate(Expression<Func<Cell, bool>> predicate) {
			return db.Cell
				.Include("Field.Module")
				.Include("Field.FieldDataType")
				.Include("Field.Metadata")
				.Include(rf => rf.Row)
				.FirstOrDefault(predicate);
		}

		public ObjectQuery<Row> GetRows() {
			return db.Row;
		}

		/// <summary>
		/// Gets the field data type object that matches the provided name
		/// </summary>
		/// <param name="_dataTypeName">Name of the data type.</param>
		public FieldDataType GetDataTypeByName(string _dataTypeName) {
			if (String.IsNullOrWhiteSpace(_dataTypeName)) {
				throw new ArgumentNullException("_dataTypeName");
			}

			return db.FieldDataType.FirstOrDefault(d => d.Name == _dataTypeName);
		}

        public void Save() {
            db.SaveChanges();
        }

		public Field GetFieldById(int _fieldId) {
			return db.Field.SingleOrDefault(f => f.ID == _fieldId);
		}

		public User GetUserById(Guid _userId) {
			return db.User.SingleOrDefault(u => u.UserId == _userId);
		}

		public User GetUserByName(string _userName) {
			return db.User.SingleOrDefault(u => u.UserName == _userName);
		}

		public IEnumerable<User> GetAccountUsers(int _accountId) {
			return db.Account
				.Include(a => a.Users)
				.SingleOrDefault(a => a.Id == _accountId)
				.Users;
		}

		public IEnumerable<Account> GetAllAccounts() {
			return db.Account.ToList();
		}

		public IEnumerable<Account> GetUserAccounts() {
			var user = Membership.GetUser();

			if (user == null) {
				return new List<Account>();
			}

			if (!Roles.IsUserInRole(user.UserName, "sysadmin")) {
				return GetUserAccounts((Guid)user.ProviderUserKey);
			}
			else {
				// system admin sees all system accounts
				return db.Account.Include(a => a.Users).ToList();
			}
		}

		public IEnumerable<Account> GetUserAccounts(Guid userId) {
			var user = db.User.Include(u => u.Accounts).FirstOrDefault(u => u.UserId == userId);

			if (user == null) {
				throw new ArgumentException("User not found");
			}

			return user.Accounts;
		}

		public Account GetAccountById(int id) {
			return db.Account.Include(acc => acc.Users).SingleOrDefault(a => a.Id == id);
		}

		public Account GetAccountByName(string subdomain) {
			return db.Account.Include(acc => acc.Users).SingleOrDefault(a => a.SubdomainName == subdomain);
		}

		public void AddAccount(Account acct) {
			db.AddToAccount(acct);
		}

		public void DeleteAccount(int id) {
			var acct = GetAccountById(id);

			db.DeleteObject(acct);
		}

		public IEnumerable<User> GetAllUsers() {
			return db.User.Include(u => u.Accounts);
		}

		public void DeleteObject(object entity) {
			db.DeleteObject(entity);
		}

		/// <summary>
		/// Increments the API requests count for the provided module.
		/// </summary>
		/// <param name="moduleId">The module id.</param>
		public void IncrementModuleRequestCount(int moduleId) {
			var dtNow = DateTime.Now;
			var module = db.Module.Include(m => m.UsageCounters).SingleOrDefault(m => m.Id == moduleId);

			if (module == null) {
				throw new ArgumentException("moduleId");
			}

			var counter = module.UsageCounters.FirstOrDefault(uc => uc.Day == dtNow.Day && uc.Month == dtNow.Month && uc.Year == dtNow.Year);

			if (counter != null) {
				counter.RequestCount++;
			}
			else {
				module.UsageCounters.Add(new UsageCounter {
					Month = (byte)dtNow.Month,
					Year = (short)dtNow.Year,
					Day = (byte)dtNow.Day,
					RequestCount = 1
				});
			}

			db.SaveChanges();
		}

		/// <summary>
		/// Gets the total number of API requests made to the provided module in the current day, month and year.
		/// </summary>
		/// <param name="moduleId">The module id.</param>
		/// <returns>The request count</returns>
		public int GetModuleRequestCount(int moduleId, int? year = null, int? month = null, int? day = null) {
			var dtNow = DateTime.Now;

			if (year == null) {
				year = dtNow.Year;
			}
			if (month == null) {
				month = dtNow.Month;
			}
			if (day == null) {
				day = dtNow.Day;
			}

			var module = db.Module.Include(m => m.UsageCounters).SingleOrDefault(m => m.Id == moduleId);

			if (module == null) {
				throw new ArgumentException("moduleId");
			}

			var counter = module.UsageCounters.FirstOrDefault(uc => uc.Day == day && uc.Month == month && uc.Year == year);

			return (counter != null) ? counter.RequestCount : 0;
		}

		public IEnumerable<UsageCounter> GetCountersByAccountId(int acctId) {
			return db.UsageCounter
				.Include("Module.Account")
				.Where(uc => uc.Module.Account.Id == acctId);
		}

		public Row GetRowById(int _rowId) {
			return db.Row.SingleOrDefault(r => r.ID == _rowId);
		}

        #region IDisposable Members

        public void Dispose() {
            db.Dispose();
        }

        #endregion
    }
}