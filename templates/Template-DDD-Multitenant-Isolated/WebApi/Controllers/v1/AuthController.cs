using Asp.Versioning;
using IAM.Application.Features.Auth.Commands.CerrarSesion;
using IAM.Application.Features.Auth.Commands.IniciarSesion;
using IAM.Application.Features.Auth.Commands.RefrescarToken;
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
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] IniciarSesionCommand command) =>
        HandleResult(await mediator.Send(command));

    [HttpPost("refresh-token")]
    [AllowAnonymous]
    public async Task<IActionResult> RefreshToken([FromBody] RefrescarTokenCommand command) =>
        HandleResult(await mediator.Send(command));

    [HttpPost("logout")]
    [AllowAnonymous]
    public async Task<IActionResult> Logout([FromBody] CerrarSesionCommand command) =>
        HandleResult(await mediator.Send(command));

    [HttpGet]
    public async Task<IActionResult> ObtenerSesion() => HandleResult(await mediator.Send(new GetSessionQuery()));
    
    [HttpPost]
    [Route("usuario")]
    public async Task<IActionResult> Post([FromBody] CreateUserCommand command) =>
        HandleResult(await mediator.Send(command));

    [HttpPut("password/{id}")]
    public async Task<IActionResult> PutPassword(Guid id, UpdateUserPasswordCommand command) =>
        HandleResult(await mediator.Send(command));

    [HttpDelete("{nombreUsuario}")]
    public async Task<IActionResult> Delete(string nombreUsuario) =>
        HandleResult(await mediator.Send(new DeleteUserCommand(nombreUsuario)));
}