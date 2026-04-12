using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace WebApi.Providers;

public class SpanishMetadataProvider : IValidationMetadataProvider, IDisplayMetadataProvider
{
    public void CreateValidationMetadata(ValidationMetadataProviderContext context)
    {
        foreach (var attribute in context.ValidationMetadata.ValidatorMetadata)
        {
            if (attribute is RequiredAttribute required)
            {
                required.ErrorMessage = "El campo {0} es obligatorio.";

                required.AllowEmptyStrings = false;
            }
        }
    }

    public void CreateDisplayMetadata(DisplayMetadataProviderContext context)
    {
        context.DisplayMetadata.ConvertEmptyStringToNull = true;
        
        var propertyName = context.Key.Name;
        if (!string.IsNullOrEmpty(propertyName))
        {
            var friendlyName = Regex.Replace(propertyName, "([a-z])([A-Z])", "$1 $2");
            
            context.DisplayMetadata.DisplayName = () => friendlyName;
        }

        context.DisplayMetadata.DisplayName = context.Key.Name switch
        {
            "UserName" => () => "Nombre de Usuario",
            "Email" => () => "Correo Electrónico",
            "Password" => () => "Contraseña",
            "Name" => () => "Nombre",
            _ => context.DisplayMetadata.DisplayName
        };
    }
}