using Microsoft.AspNetCore.Mvc;

namespace ASP.NET_API.Controllers
{
    public class CustomBaseController : ControllerBase
    {
        protected string GetUserId()
        {
            var userClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "id");
            var userId = userClaim.Value;
            return userId;
        }
    }
}
