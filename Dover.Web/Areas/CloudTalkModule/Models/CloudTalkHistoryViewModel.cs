using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Com.Dover.Areas.CloudTalkModule.Models {
	public class CloudTalkHistoryViewModel {
		public string SenderEmail { get; set; }
		public string Message { get; set; }
		public string TimeStamp { get; set; }
	}
}