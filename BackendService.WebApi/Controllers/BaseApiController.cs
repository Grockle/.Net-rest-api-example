using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackendService.WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class BaseApiController : ControllerBase
    {
        public string Token => HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
    }
}