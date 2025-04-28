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
    public record CreateRoleCommand(RolesDTO Role) : IRequest<Unit>;
    public class CreateRoleCommandHandler(IUserService _userService) : IRequestHandler<CreateRoleCommand, Unit>
    {
        public async Task<Unit> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
        {
            await _userService.CreateRole(request.Role);
            return Unit.Value;
        }
    }

}
