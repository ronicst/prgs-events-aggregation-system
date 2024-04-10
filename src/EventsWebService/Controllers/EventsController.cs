using EventsWebService.Dtos;
using EventsWebService.Infrastructure;
using EventsWebService.Security;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace EventsWebService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EventsController : ControllerBase
    {
        [HttpPost]
        [ApiKey]
        public ActionResult PostEvent([FromQuery]EventType type, [FromBody]object content)
        {
            if (content == null)
            {
                return this.BadRequest("Body can not be null.");
            }

            string[] result;
            try
            {
                switch (type)
                {
                    case EventType.None:
                        return this.BadRequest("Invalid type!");
                    case EventType.FileDownload:
                        result = new MessageSender().SendEvent(JsonConvert.DeserializeObject<FileDownloadDto>(content.ToString()), type.ToString());
                        if (result.Length > 0)
                        {
                            return this.BadRequest(result);
                        }
                        break;
                    case EventType.UserLogin:
                        result = new MessageSender().SendEvent(JsonConvert.DeserializeObject<UserLoginDto>(content.ToString()), type.ToString());
                        if (result.Length > 0)
                        {
                            return this.BadRequest(result);
                        }
                        break;
                    case EventType.UserLogout:
                        result = new MessageSender().SendEvent(JsonConvert.DeserializeObject<UserLogoutDto>(content.ToString()), type.ToString());
                        if (result.Length > 0)
                        {
                            return this.BadRequest(result);
                        }
                        break;
                    case EventType.UserRegistered:
                        result = new MessageSender().SendEvent(JsonConvert.DeserializeObject<UserRegisteredDto>(content.ToString()), type.ToString());
                        if (result.Length > 0)
                        {
                            return this.BadRequest(result);
                        }
                        break;
                    case EventType.ProductInstalled:
                        result = new MessageSender().SendEvent(JsonConvert.DeserializeObject<ProductInstalledDto>(content.ToString()), type.ToString());
                        if (result.Length > 0)
                        {
                            return this.BadRequest(result);
                        }
                        break;
                    case EventType.ProductUninstalled:
                        result = new MessageSender().SendEvent(JsonConvert.DeserializeObject<ProductUninstalledDto>(content.ToString()), type.ToString());
                        if (result.Length > 0)
                        {
                            return this.BadRequest(result);
                        }
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                return this.BadRequest(new { Error = "Unexpected error.", Message = ex.Message });
            }

            return this.Ok(new { Status = "Event processed successfully", Time = DateTime.UtcNow, ReferenseId = Guid.NewGuid() });
        }
    }
}