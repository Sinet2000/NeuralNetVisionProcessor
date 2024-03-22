using Microsoft.AspNetCore.Mvc;

namespace NetVisionProc.Api.Infra
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class BaseApiController : ControllerBase
    {
    }
}