using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Br.Com.Quavio.Tools.Web;
using Br.Com.Quavio.Tools.Web.Net;
using System.Diagnostics;

namespace Com.Dovercms.Tests {
	[TestClass]
	public class PerformanceTests {
		[TestMethod]
		public void TestApiGetModule() {
			int totalTime = 0;
			int totalReqs = 100;
			
			for (int i = 0; i < 100; i++) {
				var meter = new Stopwatch();
				meter.Start();
				string data = NetFunctions.HttpGet("http://api.localdover.com/module/45");
				meter.Stop();

				totalTime += meter.Elapsed.Milliseconds;
				Console.WriteLine(String.Format("Request {0} took {1} milliseconds.", i, meter.Elapsed.Milliseconds));
			}

			Console.WriteLine(String.Format("Total time: {0} seconds.", totalTime / 1000));
			Console.WriteLine(String.Format("Average request took {0} milliseconds.", totalTime / totalReqs));
		}
	}
}
