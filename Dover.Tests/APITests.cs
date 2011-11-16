using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web.Mvc;
using System.Xml.Linq;
using System.Web;
using Moq;
using System.Collections.Specialized;
using System.Web.Routing;
using Com.Dover.Controllers;
using Br.Com.Quavio.Tools.Web.Net;
using System.Net;
using System.IO;
using Com.Dover.Helpers;

namespace Com.Dover.Tests {
	[TestClass]
	public class APITests {
		[TestMethod]
		public void TestQueryXml() {
			var repo = new FakeModuleRepository();
			var controller = new DynamicModuleController(repo, new FakeModuleMembership());
			var module = repo.AllModules.FirstOrDefault();
			var actionresult = controller.Query(module.Id, module.ModuleName, null);
			var filter = new ApiEndPointAttribute();
			var filterContext = new ActionExecutedContext { Result = actionresult };

			filter.OnActionExecuted(filterContext);

			Assert.IsNotNull(filterContext.Result);
			Assert.IsInstanceOfType(filterContext.Result, typeof(ContentResult));

			var xmlResult = filterContext.Result as ContentResult;

			Assert.AreEqual("text/xml", xmlResult.ContentType);
			Assert.IsFalse(String.IsNullOrWhiteSpace(xmlResult.Content));
			XDocument.Parse(xmlResult.Content);
			
			Assert.AreEqual(
				"<The-test-module-0>\r\n  <ModuleId>0</ModuleId>\r\n  <The-test-module-0>\r\n    <ID>400</ID>\r\n    <TestField0>Some data 0</TestField0>\r\n  </The-test-module-0>\r\n  <The-test-module-0>\r\n    <ID>399</ID>\r\n    <TestField0>Some data 1</TestField0>\r\n  </The-test-module-0>\r\n  <The-test-module-0>\r\n    <ID>398</ID>\r\n    <TestField0>Some data 2</TestField0>\r\n  </The-test-module-0>\r\n  <The-test-module-0>\r\n    <ID>397</ID>\r\n    <TestField0>Some data 3</TestField0>\r\n  </The-test-module-0>\r\n  <The-test-module-0>\r\n    <ID>396</ID>\r\n    <TestField0>Some data 4</TestField0>\r\n  </The-test-module-0>\r\n  <The-test-module-0>\r\n    <ID>395</ID>\r\n    <TestField0>Some data 5</TestField0>\r\n  </The-test-module-0>\r\n  <The-test-module-0>\r\n    <ID>394</ID>\r\n    <TestField0>Some data 6</TestField0>\r\n  </The-test-module-0>\r\n  <The-test-module-0>\r\n    <ID>393</ID>\r\n    <TestField0>Some data 7</TestField0>\r\n  </The-test-module-0>\r\n  <The-test-module-0>\r\n    <ID>392</ID>\r\n    <TestField0>Some data 8</TestField0>\r\n  </The-test-module-0>\r\n  <The-test-module-0>\r\n    <ID>391</ID>\r\n    <TestField0>Some data 9</TestField0>\r\n  </The-test-module-0>\r\n  <The-test-module-0>\r\n    <ID>390</ID>\r\n    <TestField0>Some data 10</TestField0>\r\n  </The-test-module-0>\r\n  <The-test-module-0>\r\n    <ID>389</ID>\r\n    <TestField0>Some data 11</TestField0>\r\n  </The-test-module-0>\r\n  <The-test-module-0>\r\n    <ID>388</ID>\r\n    <TestField0>Some data 12</TestField0>\r\n  </The-test-module-0>\r\n  <The-test-module-0>\r\n    <ID>387</ID>\r\n    <TestField0>Some data 13</TestField0>\r\n  </The-test-module-0>\r\n  <The-test-module-0>\r\n    <ID>386</ID>\r\n    <TestField0>Some data 14</TestField0>\r\n  </The-test-module-0>\r\n  <The-test-module-0>\r\n    <ID>385</ID>\r\n    <TestField0>Some data 15</TestField0>\r\n  </The-test-module-0>\r\n  <The-test-module-0>\r\n    <ID>384</ID>\r\n    <TestField0>Some data 16</TestField0>\r\n  </The-test-module-0>\r\n  <The-test-module-0>\r\n    <ID>383</ID>\r\n    <TestField0>Some data 17</TestField0>\r\n  </The-test-module-0>\r\n  <The-test-module-0>\r\n    <ID>382</ID>\r\n    <TestField0>Some data 18</TestField0>\r\n  </The-test-module-0>\r\n  <The-test-module-0>\r\n    <ID>381</ID>\r\n    <TestField0>Some data 19</TestField0>\r\n  </The-test-module-0>\r\n</The-test-module-0>",
				xmlResult.Content);
		}

		[TestMethod]
		public void TestQueryById() {
			var repo = new FakeModuleRepository();
			var controller = new DynamicModuleController(repo, new FakeModuleMembership());
			var module = repo.AllModules.FirstOrDefault();
			var actionresult = controller.Query(module.Id, module.ModuleName, 400);
			var filter = new ApiEndPointAttribute();
			var filterContext = new ActionExecutedContext { Result = actionresult };

			filter.OnActionExecuted(filterContext);

			Assert.IsNotNull(filterContext.Result);
			Assert.IsInstanceOfType(filterContext.Result, typeof(ContentResult));

			var xmlResult = filterContext.Result as ContentResult;

			Assert.AreEqual("text/xml", xmlResult.ContentType);
			Assert.IsFalse(String.IsNullOrWhiteSpace(xmlResult.Content));
			XDocument.Parse(xmlResult.Content);

			Assert.AreEqual(
				"<The-test-module-0>\r\n  <ModuleId>0</ModuleId>\r\n  <The-test-module-0>\r\n    <ID>400</ID>\r\n    <TestField0>Some data 0</TestField0>\r\n  </The-test-module-0>\r\n</The-test-module-0>",
				xmlResult.Content);
		}

		[TestMethod]
		public void TestQueryByIdNotFound() {
			var repo = new FakeModuleRepository();
			var controller = new DynamicModuleController(repo, new FakeModuleMembership());
			var module = repo.AllModules.FirstOrDefault();
			var actionresult = controller.Query(module.Id, module.ModuleName, 490);
			var filter = new ApiEndPointAttribute();
			var filterContext = new ActionExecutedContext { Result = actionresult };

			filter.OnActionExecuted(filterContext);

			Assert.IsNotNull(filterContext.Result);
			Assert.IsInstanceOfType(filterContext.Result, typeof(ContentResult));

			var xmlResult = filterContext.Result as ContentResult;

			Assert.AreEqual("text/xml", xmlResult.ContentType);
			Assert.IsFalse(String.IsNullOrWhiteSpace(xmlResult.Content));
			XDocument.Parse(xmlResult.Content);

			Assert.AreEqual(
				"<The-test-module-0>\r\n  <ModuleId>0</ModuleId>\r\n</The-test-module-0>",
				xmlResult.Content);
		}

		[TestMethod]
		public void TestQueryByColumn() {
			var repo = new FakeModuleRepository();
			var controller = new DynamicModuleController(repo, new FakeModuleMembership());
			var request = new Mock<HttpRequestBase>();
			var context = new Mock<HttpContextBase>();
			var queryString = new NameValueCollection();
			
			queryString.Add("TestField0", "Some data 7");
			request.SetupGet(x => x.QueryString).Returns(queryString);
			context.SetupGet(x => x.Request).Returns(request.Object);
			controller.ControllerContext = new ControllerContext(context.Object, new RouteData(), controller);

			var module = repo.AllModules.FirstOrDefault();
			var actionresult = controller.Query(module.Id, module.ModuleName, null);
			var filter = new ApiEndPointAttribute();
			var filterContext = new ActionExecutedContext { Result = actionresult };

			filter.OnActionExecuted(filterContext);

			Assert.IsNotNull(filterContext.Result);
			Assert.IsInstanceOfType(filterContext.Result, typeof(ContentResult));

			var xmlResult = filterContext.Result as ContentResult;

			Assert.AreEqual("text/xml", xmlResult.ContentType);
			Assert.IsFalse(String.IsNullOrWhiteSpace(xmlResult.Content));
			XDocument.Parse(xmlResult.Content);

			Assert.AreEqual(
				"<The-test-module-0>\r\n  <ModuleId>0</ModuleId>\r\n  <The-test-module-0>\r\n    <ID>393</ID>\r\n    <TestField0>Some data 7</TestField0>\r\n  </The-test-module-0>\r\n</The-test-module-0>",
				xmlResult.Content);
		}

		[TestMethod]
		public void TestQueryByColumnMismatchValue() {
			var repo = new FakeModuleRepository();
			var controller = new DynamicModuleController(repo, new FakeModuleMembership());
			var request = new Mock<HttpRequestBase>();
			var context = new Mock<HttpContextBase>();
			var queryString = new NameValueCollection();

			queryString.Add("TestField0", "Some data 900");
			request.SetupGet(x => x.QueryString).Returns(queryString);
			context.SetupGet(x => x.Request).Returns(request.Object);
			controller.ControllerContext = new ControllerContext(context.Object, new RouteData(), controller);

			var module = repo.AllModules.FirstOrDefault();
			var actionresult = controller.Query(module.Id, module.ModuleName, null);
			var filter = new ApiEndPointAttribute();
			var filterContext = new ActionExecutedContext { Result = actionresult };

			filter.OnActionExecuted(filterContext);

			Assert.IsNotNull(filterContext.Result);
			Assert.IsInstanceOfType(filterContext.Result, typeof(ContentResult));

			var xmlResult = filterContext.Result as ContentResult;

			Assert.AreEqual("text/xml", xmlResult.ContentType);
			Assert.IsFalse(String.IsNullOrWhiteSpace(xmlResult.Content));
			XDocument.Parse(xmlResult.Content);

			Assert.AreEqual(
				"<The-test-module-0>\r\n  <ModuleId>0</ModuleId>\r\n</The-test-module-0>",
				xmlResult.Content);
		}

		[TestMethod]
		public void TestQueryByColumnMismatchColumnName() {
			var repo = new FakeModuleRepository();
			var controller = new DynamicModuleController(repo, new FakeModuleMembership());
			var request = new Mock<HttpRequestBase>();
			var context = new Mock<HttpContextBase>();
			var queryString = new NameValueCollection();

			queryString.Add("TestField12", "Some data 8");
			request.SetupGet(x => x.QueryString).Returns(queryString);
			context.SetupGet(x => x.Request).Returns(request.Object);
			controller.ControllerContext = new ControllerContext(context.Object, new RouteData(), controller);

			var module = repo.AllModules.FirstOrDefault();
			var actionresult = controller.Query(module.Id, module.ModuleName, null);
			var filter = new ApiEndPointAttribute();
			var filterContext = new ActionExecutedContext { Result = actionresult };

			filter.OnActionExecuted(filterContext);

			Assert.IsNotNull(filterContext.Result);
			Assert.IsInstanceOfType(filterContext.Result, typeof(ContentResult));

			var xmlResult = filterContext.Result as ContentResult;

			Assert.AreEqual("text/xml", xmlResult.ContentType);
			Assert.IsFalse(String.IsNullOrWhiteSpace(xmlResult.Content));
			XDocument.Parse(xmlResult.Content);

			Assert.AreEqual(
				"<The-test-module-0>\r\n  <ModuleId>0</ModuleId>\r\n</The-test-module-0>",
				xmlResult.Content);
		}

		[TestMethod]
		public void TestQueryByColumnMultipleResults() {
			var repo = new FakeModuleRepository();
			var controller = new DynamicModuleController(repo, new FakeModuleMembership());
			var request = new Mock<HttpRequestBase>();
			var context = new Mock<HttpContextBase>();
			var queryString = new NameValueCollection();

			queryString.Add("TestField0", "1");
			request.SetupGet(x => x.QueryString).Returns(queryString);
			context.SetupGet(x => x.Request).Returns(request.Object);
			controller.ControllerContext = new ControllerContext(context.Object, new RouteData(), controller);

			var module = repo.AllModules.FirstOrDefault();
			var actionresult = controller.Query(module.Id, module.ModuleName, null);
			var filter = new ApiEndPointAttribute();
			var filterContext = new ActionExecutedContext { Result = actionresult };

			filter.OnActionExecuted(filterContext);

			Assert.IsNotNull(filterContext.Result);
			Assert.IsInstanceOfType(filterContext.Result, typeof(ContentResult));

			var xmlResult = filterContext.Result as ContentResult;

			Assert.AreEqual("text/xml", xmlResult.ContentType);
			Assert.IsFalse(String.IsNullOrWhiteSpace(xmlResult.Content));
			XDocument.Parse(xmlResult.Content);

			Assert.AreEqual(
				"<The-test-module-0>\r\n  <ModuleId>0</ModuleId>\r\n  <The-test-module-0>\r\n    <ID>399</ID>\r\n    <TestField0>Some data 1</TestField0>\r\n  </The-test-module-0>\r\n  <The-test-module-0>\r\n    <ID>390</ID>\r\n    <TestField0>Some data 10</TestField0>\r\n  </The-test-module-0>\r\n  <The-test-module-0>\r\n    <ID>389</ID>\r\n    <TestField0>Some data 11</TestField0>\r\n  </The-test-module-0>\r\n  <The-test-module-0>\r\n    <ID>388</ID>\r\n    <TestField0>Some data 12</TestField0>\r\n  </The-test-module-0>\r\n  <The-test-module-0>\r\n    <ID>387</ID>\r\n    <TestField0>Some data 13</TestField0>\r\n  </The-test-module-0>\r\n  <The-test-module-0>\r\n    <ID>386</ID>\r\n    <TestField0>Some data 14</TestField0>\r\n  </The-test-module-0>\r\n  <The-test-module-0>\r\n    <ID>385</ID>\r\n    <TestField0>Some data 15</TestField0>\r\n  </The-test-module-0>\r\n  <The-test-module-0>\r\n    <ID>384</ID>\r\n    <TestField0>Some data 16</TestField0>\r\n  </The-test-module-0>\r\n  <The-test-module-0>\r\n    <ID>383</ID>\r\n    <TestField0>Some data 17</TestField0>\r\n  </The-test-module-0>\r\n  <The-test-module-0>\r\n    <ID>382</ID>\r\n    <TestField0>Some data 18</TestField0>\r\n  </The-test-module-0>\r\n  <The-test-module-0>\r\n    <ID>381</ID>\r\n    <TestField0>Some data 19</TestField0>\r\n  </The-test-module-0>\r\n</The-test-module-0>",
				xmlResult.Content);
		}

		[TestMethod]
		public void TestPutNewRecord() {
			string url = "http://api.localdover.com/quavio/module/75";
			string parameters = "Nome=Fulano de Tal&Email=fulano@gmail.com&Area-de-Atuacao=Informática&Data-de-Nascimento=26/03/1985&Cidade=Porto Alegre&UF=RS";

			var webRequest = (HttpWebRequest)WebRequest.Create(url);

			webRequest.ContentType = "application/x-www-form-urlencoded";
			webRequest.Method = "PUT";

			byte[] bytes = Encoding.UTF8.GetBytes(parameters);
			webRequest.ContentLength = bytes.Length;

			using (Stream outputStream = webRequest.GetRequestStream()) {
				outputStream.Write(bytes, 0, bytes.Length);
			}

			using (WebResponse webResponse = webRequest.GetResponse()) {
				if (webResponse == null) {
					Assert.Fail();
				}
				using (StreamReader sr = new StreamReader(webResponse.GetResponseStream())) {
					Assert.AreEqual("<result>Success</result>", sr.ReadToEnd().Trim());
				}
			}
		}
	}
}
