using ASC.Web.Configuration;
using ASC.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using ASC.Utilities;
namespace ASC.Web.Controllers
{
    public class HomeController : AnonymousController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IOptions<ApplicationSettings> _settings;

        public HomeController(
            ILogger<HomeController> logger,
            IOptions<ApplicationSettings> settings)
        {
            _logger = logger;
            _settings = settings;
        }

        public IActionResult Index()
        {
            HttpContext.Session.SetSession("ApplicationTitle", _settings.Value.ApplicationTitle);

            ViewBag.Title = _settings.Value.ApplicationTitle;

            return View();
        }
        [HttpGet]
        public IActionResult GetStarted()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Dashboard", "Dashboard", new { area = "ServiceRequests" });
            }

            return Redirect("/Identity/Account/Login");
        }
        public IActionResult Dashboard()
        {
            ViewBag.Title = _settings.Value.ApplicationTitle;
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }


    }
}
