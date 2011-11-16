using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Com.Dover.Modules;
using System.Web.Security;
using System.Data.Objects.DataClasses;
using Br.Com.Quavio.Tools.Web;
using System.Linq.Expressions;
using Com.Dover.Web.Models.Converters;
using Com.Dover.Web.Models;
using System.Data.Objects;

namespace Com.Dover.Tests {
    class FakeModuleRepository : IModuleRepository {

		public List<IModule> AllModules { get; private set; }
		public List<Row> AllRows { get; private set; }
        
        public FakeModuleRepository() {

			AllModules = new List<IModule>();
			AllRows = new List<Row>();
			var converter = FieldValueConversion.GetConverter(Type.GetType("System.String"));
			int j = 100;
			int k = 200;
			int l = 300;
			int m = 400;
            for(int i = 0; i < 101; i++) {
				var module = new Module() {
					DisplayName = "Test Module " + i,
					ModuleName = "The-test-module-" + i,
					Id = i,
					ModuleType = i % 3,
					User = new User() { UserId = FakeModuleMembership.FakeUserId }
				};

                var field = new Field() {
                    DisplayName = "Test field " + i,
                    FieldName = "TestField" + i,
                    ID = k--,
                    ShowInListMode = true,
                    IsRequired = true,
                    FieldDataType = new FieldDataType() {
                        Name = "System.String",
                        ID = l--,
                        FriendlyName = "Test data type " + i
                    }
                };
                
                module.Fields.Add(field);

				for (int z = 0; z < 20; z++) {
					module.Rows.Add(new Row() {
						ID = m--,
						Cells = new EntityCollection<Cell>() {
                        new Cell() { 
                            Data = converter.Serialize(new DynamicModuleField { Data = "Some data " + z }), 
                            ID = j--,
                            Field = field
						}}
					});
				}
                
                AllModules.Add(module);
				AllRows.AddRange(AllModules.FirstOrDefault().Rows);
            }
        }

		internal Cell GetRandomCell() {
			var randomModule = AllModules.ElementAt(new Random().Next(AllModules.Count - 1));
			var randomRow = randomModule.Rows.ElementAt(new Random().Next(randomModule.Rows.Count - 1));
			return randomRow.Cells.FirstOrDefault();
		}

        #region IModuleRepository Members

        public IModule GetStaticModuleByName(string _sName) {
            return AllModules
                .Where(m => m.ModuleType == (int)ModuleType.Static)
                .FirstOrDefault(m => m.ModuleName == _sName);
        }

        public IModule GetStaticModuleById(int _id) {
            return AllModules
                .Where(m => m.ModuleType == (int)ModuleType.Static)
                .FirstOrDefault(m => m.Id == _id);
        }

		public Field GetFieldById(int _fieldId) {
			throw new NotImplementedException();
		}

        public IModule GetModuleByName(string _sModuleName) {
            return AllModules.FirstOrDefault(m => m.ModuleName == _sModuleName);
        }

		public IModule GetModuleById(int _id, params Expression<Func<Module, object>>[] subSelectors) {
            return AllModules.FirstOrDefault(m => m.Id == _id);
        }

        public void RemoveUserModule(Guid _userUuid, int _moduleId) {
            throw new NotImplementedException();
        }

        public void AddUserModule(Guid _userUuid, int _moduleId) {
            throw new NotImplementedException();
        }

        public IEnumerable<IModule> GetUserModules(Guid _userId) {
            return AllModules.Where(m => m.User.UserId == _userId);
        }

        public IEnumerable<IModule> GetAllStaticModules() {
            return AllModules.Where(m => m.ModuleType == (int)ModuleType.Static);
        }

        public IEnumerable<IModule> GetUserModules() {
			return AllModules.Where(m => m.User.UserId == (Guid)Membership.GetUser().ProviderUserKey);
        }

        public void AddModuleEntry(int _moduleId, Row _entry) {
            throw new NotImplementedException();
        }

		public FieldDataType GetDataTypeByName(string _dataTypeName) {
			throw new NotImplementedException();
		}

        public void Save() {
            throw new NotImplementedException();
        }

		public void AddModule(Module _module) {
			throw new NotImplementedException();
		}

		public Row GetRowByPredicate(Expression<Func<Row, bool>> predicate) {
			var func = predicate.Compile();
			Row foundRow = null;

			AllModules.ForEach(m => {
				var row = m.Rows.FirstOrDefault(func);
				if (row != null) {
					foundRow = row;
					return;
				}
			});

			return foundRow;
		}

		public Cell GetCellByPredicate(Expression<Func<Cell, bool>> predicate) {
			throw new NotImplementedException();
		}

		public ObjectQuery<Row> GetRows() {
			throw new NotImplementedException();
		}

		public User GetUserById(Guid _id) {
			throw new NotImplementedException();
		}

		public User GetUserByName(string _userName) {
			throw new NotImplementedException();
		}

		public IEnumerable<User> GetAccountUsers(int _accountId) {
			throw new NotImplementedException();
		}

		public IEnumerable<Account> GetUserAccounts(Guid userId) {
			throw new NotImplementedException();
		}

		public Account GetAccountById(int id) {
			throw new NotImplementedException();
		}

		public Account GetAccountByName(string subdomain) {
			throw new NotImplementedException();
		}

		public void AddAccount(Account a) {
			throw new NotImplementedException();
		}

		public void DeleteAccount(int id) {
			throw new NotImplementedException();
		}

		public IEnumerable<User> GetAllUsers() {
			throw new NotImplementedException();
		}

		public IEnumerable<Account> GetUserAccounts() {
			throw new NotImplementedException();
		}

		public IEnumerable<IModule> GetAccountModules() {
			throw new NotImplementedException();
		}

		public IEnumerable<IModule> GetAccountModules(string accountName) {
			throw new NotImplementedException();
		}

		public void IncrementModuleRequestCount(int moduleId) {
			throw new NotImplementedException();
		}

		public int GetModuleRequestCount(int moduleId, int? year = null, int? month = null, int? day = null) {
			throw new NotImplementedException();
		}

		public IEnumerable<UsageCounter> GetCountersByAccountId(int acctId) {
			throw new NotImplementedException();
		}

		public Row GetRowById(int _rowId) {
			throw new NotImplementedException();
		}

		public IEnumerable<FieldDataType> GetFieldDataTypes() {
			throw new NotImplementedException();
		}

		public void DeleteObject(object entity) {
			throw new NotImplementedException();
		}

		public void AddField(Field _field) {
			throw new NotImplementedException();
		}

		public void AddCell(Cell _field) {
			throw new NotImplementedException();
		}

		public IEnumerable<Account> GetAllAccounts() {
			throw new NotImplementedException();
		}

		#endregion
    }
}
