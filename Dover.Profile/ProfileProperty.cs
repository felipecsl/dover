using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Com.Dover.Profile {
	public class ProfileProperty {
		public ProfileProperty(
			string _sLabel,
			string _sKey,
			string _sValue)
			: this(_sLabel, _sKey, _sValue, ProfilePropertyDataType.String) {
		}

		public ProfileProperty(
			string _sLabel,
			string _sKey,
			string _sValue,
			ProfilePropertyDataType _eType) {
			this.Label = _sLabel;
			this.Key = _sKey;
			this.Value = _sValue;
			this.DataType = _eType;
		}

		public string Label { get; set; }
		public string Key { get; set; }
		public string Value { get; set; }
		public ProfilePropertyDataType DataType { get; set; }
	}

	public enum ProfilePropertyDataType {
		String,
		Enumeration
	}
}