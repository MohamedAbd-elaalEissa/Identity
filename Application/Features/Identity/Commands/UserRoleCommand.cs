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
    public record UserRoleCommand(UserRolesDTO UserRoles) : IRequest<Unit>;
    public class UserRoleCommandHandler(IUserService _userService) : IRequestHandler<UserRoleCommand, Unit>
    {
        public async Task<Unit> Handle(UserRoleCommand request, CancellationToken cancellationToken)
        {
            await _userService.UserRole(request.UserRoles);
            return Unit.Value;
        }
    }
}
