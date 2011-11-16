using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Data.Objects;

namespace Com.Dover.Modules {
    public interface IModuleRepository {
        IModule GetStaticModuleByName(string _sName);
        IModule GetStaticModuleById(int _id);
        IModule GetModuleByName(string _sModuleName);
		IModule GetModuleById(int _id, params Expression<Func<Module, object>>[] subSelectors);

		User GetUserByName(string _userName);
		User GetUserById(Guid _userId);

		Row GetRowById(int _rowId);
		Field GetFieldById(int _fieldId);
		Row GetRowByPredicate(Expression<Func<Row, bool>> predicate);
		Cell GetCellByPredicate(Expression<Func<Cell, bool>> predicate);
		FieldDataType GetDataTypeByName(string _dataTypeName);
		ObjectQuery<Row> GetRows();
		
        void RemoveUserModule(Guid _userUuid, int _moduleId);
        void AddUserModule(Guid _userUuid, int _moduleId);
		void AddModule(Module _module);
        void AddModuleEntry(int _moduleId, Row _entry);
		void AddField(Field _field);
		void AddCell(Cell _field);
		void AddAccount(Account acct);
		void DeleteAccount(int id);
		void DeleteObject(object entity);
        void Save();

		IEnumerable<IModule> GetUserModules(Guid _userId);
        IEnumerable<IModule> GetAllStaticModules();
        IEnumerable<IModule> GetUserModules();
		IEnumerable<IModule> GetAccountModules();
		IEnumerable<IModule> GetAccountModules(string accountName);

		IEnumerable<User> GetAllUsers();
		IEnumerable<Account> GetAllAccounts();
		IEnumerable<Account> GetUserAccounts();
		IEnumerable<FieldDataType> GetFieldDataTypes();
		IEnumerable<User> GetAccountUsers(int _accountId);
		IEnumerable<Account> GetUserAccounts(Guid userId);

		Account GetAccountById(int id);
		Account GetAccountByName(string subdomain);

		void IncrementModuleRequestCount(int moduleId);
		int GetModuleRequestCount(int moduleId, int? year = null, int? month = null, int? day = null);
		IEnumerable<UsageCounter> GetCountersByAccountId(int acctId);
	}
}
