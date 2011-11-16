using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Com.Dover.Helpers;
using Com.Dover.Web.Models.Converters;
using System.Collections.Specialized;
using Com.Dover.Modules;
using System.Xml;
using System.Xml.Schema;
using Br.Com.Quavio.Tools.Web;
using Com.Dover.Attributes;

namespace Com.Dover.Web.Models.DataTypes {
	
	[Serializable]
	[FieldValueConverter(ConverterType = typeof(ModuleReferenceConverter))]
	public class ModuleReference : IFieldDataType {

		[NonSerialized]
		private IModuleRepository repo;
		public ModuleReference() : this(new ModuleRepository()) {
		}

		public ModuleReference(IModuleRepository _repo) {
			repo = _repo;
		}
		
		public int Id { get; set; }
		public int ModuleId { get; set; }
		public List<ModuleReferenceEntry> AllItems { 
			get {
                // lazy load referenced items
				if(allItems == null) {
					allItems = new List<ModuleReferenceEntry>();

					var module = repo.GetModuleById(ModuleId,
						m => m.Rows.Include<Row, Cell>(r => r.Cells),
						m => m.Fields.Include<Field, FieldDataType>(f => f.FieldDataType));

					if (module == null) {
						// referenced module could not be found
						return allItems;
					}

					foreach (Row _row in module.Rows) {
						// show only the first visible field in the referenced module
						var rowField = _row.Cells.FirstOrDefault(rowfield => rowfield.Field.ShowInListMode && rowfield.Field.FieldDataType.Name != typeof(ModuleReference).FullName);

						if (rowField != null) {
							if (rowField.Data != null) {
								var converter = FieldValueConversion.GetConverter(Type.GetType(rowField.Field.FieldDataType.Name));
								allItems.Add(new ModuleReferenceEntry {
									Id = _row.ID,
									Value = converter.Deserialize(rowField, new ConversionContext {
										Module = module,
										Repository = repo,
										Field = rowField.Field
									}).ToString()
								});
							}
							else {
								allItems.Add(new ModuleReferenceEntry {
									Id = -1,
									Value = "<campo vazio>"
								});
							}
						}
					}
                }
                return allItems;
            }
            set { allItems = value; }
		}
		[NonSerialized]
		private List<ModuleReferenceEntry> allItems;

		public string SelectedItem {
			get {
				if (AllItems != null) {
					var selItem = AllItems.FirstOrDefault(e => e.Id == Id);

					return selItem != null ? selItem.Value : null;
				}

				var rowField = repo.GetCellByPredicate(rf => rf.Row.Module.Id == ModuleId && rf.Row.ID == Id);

				if (rowField == null ||
					rowField.Data == null) {
					return "Nenhum registro encontrado.";
				}
			
				var converter = FieldValueConversion.GetConverter(Type.GetType(rowField.Field.FieldDataType.Name));
				return converter.Deserialize(rowField, new ConversionContext {
					Module = rowField.Row.Module,
					Repository = repo,
					Field = rowField.Field
				}).ToString();
			}
		}

		#region IXmlSerializable Members

		public XmlSchema GetSchema() {
			return null;
		}

		public void ReadXml(XmlReader reader) {
			if (reader.IsEmptyElement) {
				reader.Read();
				return;
			}
			reader.ReadStartElement("ModuleReference");
			int result;
			if (Int32.TryParse(reader.ReadString(), out result)) {
				Id = result;
			}
			reader.ReadEndElement();
		}

		public void WriteXml(System.Xml.XmlWriter writer) {
			writer.WriteString(ToString());
		}

		#endregion

		public override string ToString() {
			return Id.ToString();
		}
	}

	public class ModuleReferenceEntry {
		public int Id { get; set; }
		public string Value { get; set; }
	}
}