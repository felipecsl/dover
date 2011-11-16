using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Com.Dover.Web.Models.DataTypes;
using Com.Dover.Modules;
using System.Web.Script.Serialization;
using Br.Com.Quavio.Tools.Web;

namespace Com.Dover.Web.Models.Converters {
	public class CheckBoxListConverter : DefaultFieldValueConverter {
		public override string Serialize(DynamicModuleField _obj, ConversionContext _context = null) {
			return base.Serialize(_obj);
		}

		public override object Deserialize(Cell _data, ConversionContext _context = null) {
			object data = base.Deserialize(_data);
			CheckBoxList chkList;

			if (data == null) {
				chkList = new CheckBoxList();
			}
			else {
				chkList = data as CheckBoxList;
			}

			if (_context == null ||
				_context.Field == null ||
				_context.Field.Metadata == null) {
				return chkList;
			}

			if (!_context.Field.Metadata.IsLoaded) {
				_context.Field.Metadata.Load();
			}

			var metadata = _context.Field.Metadata.FirstOrDefault(m => m.Key == ModuleRepository.CheckBoxListMetadataKey);

			if (metadata != null &&
				!String.IsNullOrWhiteSpace(metadata.Value)) {

				var json = new JavaScriptSerializer();
				var items = json.Deserialize<string[]>(metadata.Value);

				if (items != null) {
					foreach (var i in items) {
						// avoid adding empty entries to the dictionary
						if (!String.IsNullOrWhiteSpace(i)) {
							var newItem = new CheckBoxItem {
								Key = i.EscapeName(),
								Value = i
							};
							
							// checked options will already be present in the list
							if(!chkList.Any(it => it.Value == i)) {
								chkList.Add(newItem);
							}
						}
					}
				}
			}

			return chkList;
		}
	}
}