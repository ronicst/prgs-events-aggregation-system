using Microsoft.AspNetCore.Mvc;

namespace EventsWebService.Controllers
{
    [ApiController]
    public class EnvironmentController : ControllerBase
    {
        [HttpGet]
        [Route("environment")]
        public ActionResult<object> GetEnvironment()
        {
            var environmentDto = new
            {
                GtmId = "GTM-K5N5LX",
                SupportedEvents = new[] { "FileDownload", "UserLogin", "UserRegistered", "UserLogout", "ProductInstalled", "ProductUninstalled", "InvoiceCreated" }
            };

            return this.Ok(environmentDto);
        }
    }
}