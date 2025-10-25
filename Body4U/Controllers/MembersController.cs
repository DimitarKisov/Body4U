using Body4U.Membership.Application.Commands.CreateMember;
using Microsoft.AspNetCore.Mvc;

namespace Body4U.Membership.Api.Controllers
{
    public class MembersController : ApiController
    {
        [HttpPost]
        [Route(nameof(Create))]
        public async Task<ActionResult<Guid>> Create(CreateMemberCommand command)
        {
            var id =  await Mediator.Send(command);
            return CreatedAtAction("Get", new { id }, id);
        }
    }
}
