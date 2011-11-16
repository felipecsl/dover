using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Com.Dover.Web.Models {
	public class ModulesViewModel {
		public ModulesViewModel() {
			this.Fields = new List<ModuleField>();
		}

		[HiddenInput(DisplayValue = false)]
		[UIHint("HiddenInput")]
		public int Id { get; set; }

		[DisplayName("Nome do módulo:")]
		public string DisplayName { get; set; }

		[ScaffoldColumn(false)]
		public List<ModuleField> Fields { get; set; }

		[ScaffoldColumn(false)]
		public SelectList DataTypeList { get; set; }

		[ScaffoldColumn(false)]
		public string AccountName { get; set; }

		[ScaffoldColumn(false)]
		[DisplayName("Tipo:")]
		public int Type { get; set; }
	}

	public class ModuleField {
		public ModuleField() {
			ID = -1;
			ValidValues = new List<string>();
		}

		public int ID { get; set; }
		public string FieldDisplayName { get; set; }
		public string DataType { get; set; }
		public bool IsRequired { get; set; }
		public bool IsReadOnly { get; set; }
		public bool IsPendingRemoval { get; set; }
		public bool ShowInListMode { get; set; }
		public List<string> ValidValues { get; set; }
		public string Metadata { get; set; }
	}
}