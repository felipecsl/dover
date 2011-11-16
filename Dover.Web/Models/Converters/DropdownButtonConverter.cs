using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Com.Dover.Web.Models.DataTypes;
using Br.Com.Quavio.Tools.Web;
using Com.Dover.Modules;
using System.Web.Script.Serialization;

namespace Com.Dover.Web.Models.Converters {
	public class DropdownButtonConverter : DefaultFieldValueConverter {
		public override string Serialize(DynamicModuleField _obj, ConversionContext _context = null) {
			return base.Serialize(_obj);
		}

		public override object Deserialize(Cell _data, ConversionContext _context = null) {
			object data =  base.Deserialize(_data);
			DropdownButton ddButton;

			if (data == null) {
				ddButton = new DropdownButton();
			}
			else {
				ddButton = data as DropdownButton;
			}

			if (_context == null ||
				_context.Field == null ||
				_context.Field.Metadata == null) {
				return ddButton;
			}

			if (!_context.Field.Metadata.IsLoaded) {
				_context.Field.Metadata.Load();
			}

			var metadata = _context.Field.Metadata.FirstOrDefault(m => m.Key == ModuleRepository.DropdownButtonMetadataKey);

			if (metadata != null &&
				!String.IsNullOrWhiteSpace(metadata.Value)) {
				
				var json = new JavaScriptSerializer();
				var items = json.Deserialize<string[]>(metadata.Value);

				if (items != null) {
					foreach (var i in items) {
						if (!String.IsNullOrWhiteSpace(i)) {
							// avoid adding empty entries to the dictionary
							ddButton.ValidValues.Add(i.EscapeName(), i);
						}
					}
				}
			}

			return ddButton;
		}
	}
}