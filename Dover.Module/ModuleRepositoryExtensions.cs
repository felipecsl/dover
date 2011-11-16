using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Dover.Modules {
	public static class ModuleRepositoryExtensions {
		/// <summary>
		/// Adds metadata to the field object.
		/// </summary>
		/// <param name="field">The field.</param>
		/// <param name="_metadataJson">The metadata json string.</param>
		public static void AddMetadata(this Field field, string _metadataJson) {
			if (_metadataJson == null) {
				throw new ArgumentNullException("_metadataJson");
			}

			// TODO: Improve this checking as we create new field metadata types
			string metadataKey = null;

			switch (field.FieldDataType.Name) {
				case "Com.Dover.Web.Models.DataTypes.DropdownButton":
					metadataKey = ModuleRepository.DropdownButtonMetadataKey;
					break;
				case "Com.Dover.Web.Models.DataTypes.ModuleReference":
					metadataKey = ModuleRepository.ModuleReferenceMetadataKey;
					break;
				case "Com.Dover.Web.Models.DataTypes.CheckBoxList":
					metadataKey = ModuleRepository.CheckBoxListMetadataKey;
					break;
				default:
					throw new ArgumentException("field");
			}

			var metadata = field.Metadata.FirstOrDefault(m => m.Key == metadataKey);

			if (metadata != null) {
				metadata.Value = _metadataJson;
			}
			else {
				field.Metadata.Add(new FieldMetadata() {
					Key = metadataKey,
					Value = _metadataJson
				});
			}
		}
	}
}
