using System.Text.RegularExpressions;
using SharedKernel.Behaviors;

namespace IAM.Application.Features.Users.Commands.CreateUser;

public class CreateUserCommandValidator : IValidator<CreateUserCommand>
{
    public IEnumerable<string> Validate(CreateUserCommand instance)
    {
        if (!ValidarFormatoCorreo(instance.Email ?? ""))
            yield return "El correo no tiene un formato valido.";
    }
    
    private static bool ValidarFormatoCorreo(string correo)
    {
        string expresionRegular = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        return Regex.IsMatch(correo, expresionRegular);
    }
}