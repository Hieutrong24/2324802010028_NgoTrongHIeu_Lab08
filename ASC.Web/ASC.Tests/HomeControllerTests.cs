using ASC.Web.Configuration;
using ASC.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using ASC.Tests.TestUtilities;
using Microsoft.AspNetCore.Http;
using ASC.Utilities;

namespace ASC.Tests
{
    public class HomeControllerTests
    {
        private HomeController CreateHomeController()
        {
            var loggerMock = new Mock<ILogger<HomeController>>();

            var settings = new ApplicationSettings
            {
                ApplicationTitle = "Automobile Service Center Application"
            };

            var optionsMock = new Mock<IOptions<ApplicationSettings>>();
            optionsMock.Setup(x => x.Value).Returns(settings);

            var controller = new HomeController(loggerMock.Object, optionsMock.Object);

            var httpContext = new DefaultHttpContext();
            httpContext.Session = new FakeSession();

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            return controller;
        }

        [Fact]
        public void HomeController_Index_ReturnsViewResult_Test()
        {
            var controller = CreateHomeController();

            var result = controller.Index();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void HomeController_Index_ModelIsNull_Test()
        {
            var controller = CreateHomeController();

            var result = controller.Index() as ViewResult;

            Assert.Null(result?.Model);
        }

        [Fact]
        public void HomeController_Index_ModelStateIsValid_Test()
        {
            var controller = CreateHomeController();

            controller.Index();

            Assert.True(controller.ModelState.IsValid);
        }

        [Fact]
        public void HomeController_Index_Session_Test()
        {
            var controller = CreateHomeController();

            controller.Index();

            var title = controller.HttpContext.Session.GetSession<string>("ApplicationTitle");

            Assert.NotNull(title);
            Assert.Equal("Automobile Service Center Application", title);
        }
    }
}