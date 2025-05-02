using ApplicationContract;
using ApplicationContract.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Identity.Commands
{
    public record RegisterCommand(RegisterDTO Register):IRequest<string>;

    public class RegisterCommandCommandHandler(IUserService _userService) : IRequestHandler<RegisterCommand, string>
    {
        public async Task<string> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            return await _userService.RegisterAsync(request.Register);
        }
    }
}
