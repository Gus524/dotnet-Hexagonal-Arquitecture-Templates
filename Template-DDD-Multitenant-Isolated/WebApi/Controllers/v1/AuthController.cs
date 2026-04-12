using Asp.Versioning;
using IAM.Application.Features.Auth.Commands.InicioUsuario;
using IAM.Application.Features.Auth.Queries.GetSession;
using IAM.Application.Features.Users.Commands.CreateUser;
using IAM.Application.Features.Users.Commands.DeleteUser;
using IAM.Application.Features.Users.Commands.UpdateUserPassword;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Ports.In;

namespace WebApi.Controllers.v1;

[ApiVersion("1.0")]
[Authorize]
public class AuthController(IMediator mediator) : BaseApiController
{
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> AutenticarUsuario(InicioUsuarioCommand auth) => 
        HandleResult(await mediator.Send(auth));

    [HttpGet]
    public async Task<IActionResult> ObtenerSesion() => HandleResult(await mediator.Send(new GetSessionQuery()));
    
    [HttpPost]
    [Route("user")]
    public async Task<IActionResult> Post([FromBody] CreateUserCommand command) =>
        HandleResult(await mediator.Send(command));

    [HttpPut("password/{id}")]
    public async Task<IActionResult> PutPassword(Guid id, UpdateUserPasswordCommand command) =>
        HandleResult(await mediator.Send(command));

    [HttpDelete("{nombreUsuario}")]
    public async Task<IActionResult> Delete(string nombreUsuario) =>
        HandleResult(await mediator.Send(new DeleteUserCommand(nombreUsuario)));
}