using ApplicationContract;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Identity.Queries
{
    public record GetUserRolesQuery(string email) : IRequest<List<string>>;
    public class GetUserRolesQueryHandler(IUserService _userService) : IRequestHandler<GetUserRolesQuery, List<string>>
    {
        public async Task<List<string>> Handle(GetUserRolesQuery request, CancellationToken cancellationToken)
        {
            return await _userService.GetUserRoles(request.email);
        }
    }

}
