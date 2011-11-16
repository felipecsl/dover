using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web.Mvc;
using Moq;
using System.Web.Security;
using System.Web;
using System.Web.Routing;
using System.Security.Policy;
using Com.Dover.Controllers;
using Com.Dover.Web.Models;
using Com.Dover.Helpers;
using Com.Dover.Attributes;
using Com.Dover.Modules;

namespace Com.Dover.Tests.Controllers {
    [TestClass]
    public class DynamicModuleControllerTest {
        [TestMethod]
        public void Test_Dynamic_Module_List() {
            // Arrange
            var fakeRepo = new FakeModuleRepository();
            var fakeMembership = new FakeModuleMembership();
            
            DynamicModuleController controller = new DynamicModuleController(fakeRepo, fakeMembership);

            // Act
            var fakeModule = fakeRepo.GetModuleById(0);
            var result = controller.List(fakeModule);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));

            var viewResult = result as ViewResult;
            var model = viewResult.ViewData.Model;

            Assert.IsNotNull(model);
            Assert.IsInstanceOfType(model, typeof(IEnumerable<DynamicModuleViewModel>));

            var viewModel = model as IEnumerable<DynamicModuleViewModel>;

            Assert.AreEqual(fakeModule.ModuleName, viewResult.ViewData["ModuleName"]);
            Assert.AreEqual(fakeModule.DisplayName, viewResult.ViewData["DisplayName"]);
        }

        [TestMethod]
        public void Test_Dynamic_Module_Filter() {
            // Arrange
            var request = new Mock<HttpRequestBase>();
            request.Setup(r => r.HttpMethod).Returns("GET");
            request.SetupGet(r => r.Url).Returns(new Uri("http://localhost/uac/dyn/0/The-test=module-0"));

            var httpContext = new Mock<HttpContextBase>();
            httpContext.Setup(c => c.Request).Returns(request.Object);

            var routeData = new RouteData(); //
            routeData.Values.Add("moduleid", "0");

            var fakeRepo = new FakeModuleRepository();
            var fakeMembership = new FakeModuleMembership();
            var controller = new DynamicModuleController(fakeRepo, fakeMembership);

            var filterContext = new Mock<ActionExecutingContext>();
            filterContext.SetupGet(c => c.HttpContext).Returns(httpContext.Object);
            filterContext.SetupGet(c => c.RouteData).Returns(routeData);
            filterContext.SetupGet(c => c.ActionParameters).Returns(new Dictionary<string, object>());
            filterContext.Setup(c => c.Controller).Returns(controller);

            var attrib = new DynamicModuleActionAttribute();

            // Act
            attrib.OnActionExecuting(filterContext.Object);

            // Assert
            Assert.IsNotNull(filterContext.Object.ActionParameters["module"]);
            Assert.IsInstanceOfType(filterContext.Object.ActionParameters["module"], typeof(Module));
        }

        [TestMethod]
        public void Test_Dynamic_Module_Filter_Unauthorized_User() {
            // Arrange
            var request = new Mock<HttpRequestBase>();
            request.Setup(r => r.HttpMethod).Returns("GET");
            request.SetupGet(r => r.Url).Returns(new Uri("http://localhost/uac/dyn/0/The-test-module-0"));

            var httpContext = new Mock<HttpContextBase>();
            httpContext.Setup(c => c.Request).Returns(request.Object);

            var routeData = new RouteData(); //
            routeData.Values.Add("moduleid", "0");

            var fakeRepo = new FakeModuleRepository();
            var fakeMembership = new FakeModuleMembership("Fulano de Tal", "fulano@gmail.com");
            FakeModuleMembership.FakeUserId = Guid.NewGuid();   // change the user id for one that is not authorized
            var controller = new DynamicModuleController(fakeRepo, fakeMembership);

            var filterContext = new Mock<ActionExecutingContext>();
            filterContext.SetupGet(c => c.HttpContext).Returns(httpContext.Object);
            filterContext.SetupGet(c => c.RouteData).Returns(routeData);
            filterContext.SetupGet(c => c.ActionParameters).Returns(new Dictionary<string, object>());
            filterContext.Setup(c => c.Controller).Returns(controller);

			var attrib = new DynamicModuleActionAttribute() { CheckLogin = true };

            // Act
            attrib.OnActionExecuting(filterContext.Object);

            // Assert
            Assert.IsInstanceOfType(filterContext.Object.Result, typeof(RedirectToRouteResult));
            Assert.AreEqual("Usuário não autorizado.", filterContext.Object.Controller.TempData["Message"]);
            Assert.AreEqual((filterContext.Object.Result as RedirectToRouteResult).RouteValues["Controller"], "Home");
        }

		[TestMethod]
		public void Test_Dynamic_Module_Filter_Admin_User() {
			// Arrange
			var request = new Mock<HttpRequestBase>();
			request.Setup(r => r.HttpMethod).Returns("GET");
			request.SetupGet(r => r.Url).Returns(new Uri("http://localhost/uac/dyn/0/The-test-module-0"));

			var httpContext = new Mock<HttpContextBase>();
			httpContext.Setup(c => c.Request).Returns(request.Object);

			var routeData = new RouteData(); //
			routeData.Values.Add("moduleid", "0");

			var fakeRepo = new FakeModuleRepository();
			var fakeMembership = new FakeModuleMembership("Fake Admin", "admin@dovercms.com");
			FakeModuleMembership.FakeUserId = Guid.NewGuid();   // change the user id for one that is not authorized
			var controller = new DynamicModuleController(fakeRepo, fakeMembership);

			var filterContext = new Mock<ActionExecutingContext>();
			filterContext.SetupGet(c => c.HttpContext).Returns(httpContext.Object);
			filterContext.SetupGet(c => c.RouteData).Returns(routeData);
			filterContext.SetupGet(c => c.ActionParameters).Returns(new Dictionary<string, object>());
			filterContext.Setup(c => c.Controller).Returns(controller);

			var attrib = new DynamicModuleActionAttribute() { CheckLogin = true };

			// Act
			attrib.OnActionExecuting(filterContext.Object);

			// Assert
			Assert.IsNotNull(filterContext.Object.ActionParameters["module"]);
			Assert.IsInstanceOfType(filterContext.Object.ActionParameters["module"], typeof(Module));
		}
    }
}