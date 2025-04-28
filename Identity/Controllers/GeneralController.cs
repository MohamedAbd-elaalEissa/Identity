using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GeneralController : ControllerBase
    {
        private IMediator? _mediator;
        protected IMediator? Mediator => _mediator ??= HttpContext.RequestServices.GetService<IMediator>();
    }
}
