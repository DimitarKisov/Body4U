using Body4U.Membership.Application.Commands;
using Microsoft.AspNetCore.Mvc;

namespace Body4U.Membership.Api.Controllers
{
    public class MembersController : ApiController
    {
        public async Task<ActionResult<Guid>> Create(CreateMemberCommand command)
        {
            var id =  await Mediator.Send(command);
            return CreatedAtAction("Get", new { id }, id);
        }
    }
}
