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
    public record LoginCommand(LoginDTO Login):IRequest<UserDTO>;
    public class LoginCommandHandler(IUserService _userService) : IRequestHandler<LoginCommand, UserDTO>
    {
        public async Task<UserDTO> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            return await _userService.LoginAsync(request.Login);
        }
    }

}
