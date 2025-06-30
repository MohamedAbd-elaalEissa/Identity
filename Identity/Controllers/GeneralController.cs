using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class GeneralController : ControllerBase
    {
        private IMediator? _mediator;
        protected IMediator? Mediator => _mediator ??= HttpContext.RequestServices.GetService<IMediator>();
    }
}
