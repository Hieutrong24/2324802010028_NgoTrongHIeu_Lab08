using Microsoft.AspNetCore.Mvc;

namespace ASC.Web.Controllers
{
    public class ErrorController : AnonymousController
    {
        [Route("Error/{statusCode:int}")]
        public IActionResult HttpStatusCodeHandler(int statusCode)
        {
            if (statusCode == StatusCodes.Status404NotFound)
            {
                Response.StatusCode = StatusCodes.Status404NotFound;
                return View("NotFound");
            }

            Response.StatusCode = statusCode;
            ViewData["StatusCode"] = statusCode;
            return View("StatusCode");
        }
    }
}
