using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Com.Dover.Web.Models;
using System.Text;

namespace Com.Dover.Web.Helpers {
	public class CsvResult : ActionResult {
		public CsvResult() {
		}

		public DynamicModuleApiResult Data { get; set; }

		public override void ExecuteResult(ControllerContext context) {
			var itemSeparator = ";";

			var sb = new StringBuilder();

			if (Data == null) {
				return;	// do nothing
			}

			var firstRow = Data.Rows.FirstOrDefault();

			if (firstRow == null) {
				return;	// do nothing
			}

			foreach (var field in firstRow.Fields) {
				sb.Append(field.DisplayName + itemSeparator);
			}

			sb.AppendLine();

			foreach (var record in Data.Rows) {
				foreach (var field in record.Fields) {
					sb.Append((field.Data != null) ? field.Data.ToString() : String.Empty);
					sb.Append(itemSeparator);
				}
				sb.AppendLine();
			}

			var result = new ContentResult {
				ContentType = "text/csv",
				Content = sb.ToString(),
				ContentEncoding = Encoding.Default
			};

			result.ExecuteResult(context);
		}
	}
}