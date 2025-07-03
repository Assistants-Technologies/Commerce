using Microsoft.AspNetCore.Mvc;

namespace Infra.Modules.CoreIdentity.Controllers
{
    [ApiController]
    [Route("account")]
    public class AccountController : ControllerBase
    {
        [HttpGet("ping")]
        public IActionResult Ping() => Ok("CoreIdentity is alive");
    }
}