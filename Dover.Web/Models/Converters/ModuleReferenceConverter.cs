using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Com.Dover.Modules;
using Com.Dover.Web.Models.DataTypes;
using Br.Com.Quavio.Tools.Web;

namespace Com.Dover.Web.Models.Converters {
	public class ModuleReferenceConverter : DefaultFieldValueConverter {
		public override string Serialize(DynamicModuleField _obj, ConversionContext _context = null) {
			return base.Serialize(_obj);
		}

		public override object Deserialize(Cell _data, ConversionContext _context = null) {
			if (_context == null) {
				throw new ArgumentNullException("_context");
			}
			
			object data = base.Deserialize(_data);
			ModuleReference moduleRef = null;
			
			if (data == null) {
				moduleRef = new ModuleReference();
			}
			else {
				moduleRef = data as ModuleReference;

				if (moduleRef == null) {
					moduleRef = new ModuleReference();
				}
			}

			if (!_context.Field.Metadata.IsLoaded) {
				_context.Field.Metadata.Load();
			}
			// grab the module id from the field metadata
			var fieldMetadata = _context.Field.Metadata.FirstOrDefault(m => m.Key == ModuleRepository.ModuleReferenceMetadataKey);

			if (fieldMetadata == null ||
				String.IsNullOrWhiteSpace(fieldMetadata.Value)) {
				// metadata not found
				return null;
			}

			// at this point, we should have a JSON string containing the referenced module id
			int id;
			if (!Int32.TryParse(fieldMetadata.Value.Trim("\"".ToCharArray()), out id)) {
				// unable to parse the referenced module id
				return null;
			}

			moduleRef.ModuleId = id;
			
			return moduleRef;
		}
	}
}
