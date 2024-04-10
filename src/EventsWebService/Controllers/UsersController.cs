using EventsWebService.Infrastructure;
using EventsWebService.Security;
using Microsoft.AspNetCore.Mvc;

namespace EventsWebService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        [HttpDelete]
        [ApiKey]
        public ActionResult DeleteUserData([FromQuery]string userEmail)
        {
            new MessageSender().SendUserDelete(userEmail);

            return this.Ok(new { Status = "User data deleted successfully" });
        }
    }
}