namespace Body4U.Membership.Api.Controllers
{
    using Body4U.Membership.Application.Commands.CreateMember;
    using Body4U.Membership.Application.Queries.GetMemberById;
    using MediatR;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("api/[controller]")]
    public class MembersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MembersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Route(nameof(Create))]
        public async Task<ActionResult<Guid>> Create(CreateMemberCommand command)
        {
            var id =  await _mediator.Send(command);
            return CreatedAtAction(nameof(GetMember), new { id }, id);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetMember(Guid id)
        {
            try
            {
                var member = await _mediator.Send(new GetMemberByIdQuery { Id = id });
                return Ok(member);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
