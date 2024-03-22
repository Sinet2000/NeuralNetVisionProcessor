// using Microsoft.AspNetCore.Mvc;
// using NetVisionProc.Api.Infra;
//
// namespace NetVisionProc.Api.Controllers
// {
//     public class PersonController : BaseApiController
//     {
//         private readonly IPersonService _service;
//
//         public PersonController(IPersonService service)
//         {
//             _service = service;
//         }
//
//         [HttpGet]
//         public async Task<IActionResult> ListAll([FromQuery] SieveModel request, CancellationToken cancellationToken)
//         {
//             var result = await _service.List(request, cancellationToken);
//             return Ok(result);
//         }
//     }
// }