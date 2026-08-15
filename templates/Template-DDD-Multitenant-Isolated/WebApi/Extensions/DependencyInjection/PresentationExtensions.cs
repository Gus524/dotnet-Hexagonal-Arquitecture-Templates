using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi;
using SharedKernel.Ports.Out.MultiTenancy;
using SharedKernel.Ports.Out.Security;
using SharedKernel.Wrappers;
using WebApi.Providers;
using WebApi.Security;
using WebApi.Services;

namespace WebApi.Extensions.DependencyInjection;

/// <summary>
/// Centraliza la configuración de la capa de presentación (API HTTP), aislando los detalles de 
/// serialización, versionamiento, documentación (OpenAPI) y seguridad perimetral web.
/// </summary>
/// <remarks>
/// Convierte las configuraciones dispersas de ASP.NET Core en un módulo cohesivo. 
/// Posee un alto valor arquitectónico en su configuración de Controladores, donde intercepta 
/// los errores intrínsecos del framework (como fallos de binding de <c>ModelState</c>) y los traduce 
/// automáticamente al patrón Result estandarizado de la aplicación, asegurando consistencia en la respuesta.
/// </remarks>
public static class PresentationExtensions
{
    extension(IServiceCollection services)
    {
        public void AddPresentationLayer()
        {
            services.AddCustomCors();
            services.AddCustomOpenApi();
            services.AddVersioning();

            services.AddHttpContextAccessor();
            services.AddServices();
            services.AddCustomControllers();

            services.AddPresentationMappers();
        }

        private void AddCustomOpenApi()
        {
            services.AddOpenApi("v1", options =>
            {
                options.AddOperationTransformer((operation, _, _) =>
                {
                    operation.Parameters?.Add(new OpenApiParameter
                    {
                        Name = "X-Tenant-ID",
                        In = ParameterLocation.Header,
                        Required = true,
                        Description = "Identificador de sistema destino (Multitenancy)",

                    });
                    return Task.CompletedTask;
                });
                
                options.AddDocumentTransformer((document, _, _) =>
                {
                    document.Info = new OpenApiInfo
                    {
                        Title = "Template Hexagonal Architecture.",
                        Version = "v1",
                        Description = "Description of the project, this project support multitenancy different database schemes.",
                        License = new OpenApiLicense
                        {
                            Name = "Private property"
                        }
                    };

                    var securityScheme = new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.Http,
                        Scheme = "bearer",
                        In = ParameterLocation.Header,
                        BearerFormat = "Json Web Token",
                        Name = "Authorization"
                    };

                    document.Components ??= new OpenApiComponents();
                    document.Components.SecuritySchemes = new Dictionary<string, IOpenApiSecurityScheme>
                    {
                        ["Bearer"] = securityScheme
                    };

                    var schemeReference = new OpenApiSecuritySchemeReference("Bearer", document);

                    document.Security = new List<OpenApiSecurityRequirement>
                    {
                        new()
                        {
                            [schemeReference] = []
                        }
                    };

                    return Task.CompletedTask;
                });
            });
        }

        private void AddCustomCors()
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", corsBuilder =>
                {
                    corsBuilder
                        .SetIsOriginAllowed(_ => true)
                        .AllowAnyMethod()
                        .WithHeaders("Content-Type", "Authorization", "X-Tenant-ID")
                        .AllowCredentials();
                });
            });
        }

        private void AddVersioning()
        {
            services.AddApiVersioning(config =>
                {
                    config.DefaultApiVersion = new ApiVersion(1, 0);
                    config.AssumeDefaultVersionWhenUnspecified = true;
                    config.ReportApiVersions = true;
                })
                .AddApiExplorer(options =>
                {
                    options.GroupNameFormat = "'v'VVV";
                    options.SubstituteApiVersionInUrl = true;
                });
        }

        private void AddServices()
        {
            services.AddSingleton<IAuthorizationMiddlewareResultHandler, CustomAuthorizationResultHandler>();
            services.AddScoped<IUserContext, UserContext>();
            services.AddScoped<ITenantProvider, HttpHeaderTenantProvider>();
        }

        private void AddPresentationMappers()
        {
        }

        private void AddCustomControllers()
        {
            services.AddControllers(options =>
                {
                    options.ModelMetadataDetailsProviders.Add(new SpanishMetadataProvider());

                    var msgs = options.ModelBindingMessageProvider;

                    msgs.SetAttemptedValueIsInvalidAccessor((value, _) =>
                        $"El valor '{value}' no es válido.");

                    msgs.SetUnknownValueIsInvalidAccessor((field) =>
                        $"El valor proporcionado no es válido para {field}.");

                    msgs.SetValueMustNotBeNullAccessor((field) =>
                        $"El campo {field} es obligatorio.");

                    msgs.SetNonPropertyAttemptedValueIsInvalidAccessor((value) =>
                        $"El valor '{value}' no es válido.");

                    msgs.SetNonPropertyUnknownValueIsInvalidAccessor(() =>
                        "El valor proporcionado no es válido.");

                    msgs.SetValueIsInvalidAccessor((field) =>
                        $"El tipo de dato para el campo '{field}' no es correcto.");
                })
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.InvalidModelStateResponseFactory = context =>
                    {
                        var errors = context.ModelState
                            .Where(v => v.Value?.Errors.Count > 0)
                            .SelectMany(v => v.Value!.Errors)
                            .Select(e => e.ErrorMessage)
                            .ToList();

                        var response = Response.Fail<object>("Errores de validación", errors);
                        return new BadRequestObjectResult(response);
                    };
                });
        }
    }
}