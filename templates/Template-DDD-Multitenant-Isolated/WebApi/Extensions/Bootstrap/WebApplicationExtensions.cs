using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Scalar.AspNetCore;
using SharedKernel.Configuration;
using WebApi.Middlewares;

namespace WebApi.Extensions.Bootstrap;

/// <summary>
/// Ensambla el flujo de procesamiento de peticiones (Middleware Pipeline) de ASP.NET Core, 
/// dictando el orden exacto en el que las solicitudes HTTP son interceptadas, autenticadas y enrutadas.
/// </summary>
/// <remarks>
/// Oculta la complejidad de la configuración del servidor. Destaca la inclusión del middleware de 
/// captura de errores global y el enrutamiento seguro de archivos estáticos, manteniendo al 
/// archivo <c>Program.cs</c> completamente limpio de detalles de bajo nivel.
/// </remarks>
public static class WebApplicationExtensions
{
    extension(WebApplication app)
    {
        public void ConfigureRequestPipeline()
        {
            app.UseMiddleware<ErrorHandlerMiddleware>();

            if (app.Environment.IsDevelopment())
            {
                app.ConfigureApiDocumentation();
            }
            
            app.UseCustomStaticFiles();
            
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors("CorsPolicy");

            app.UseAuthentication();
            app.UseAuthorization();
            
            app.MapControllers();
        }

        private void ConfigureApiDocumentation()
        {
            app.MapOpenApi();
            app.MapScalarApiReference(o =>
            {
                o.Title = "Hexagonal Architecture API";
                o.WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
                o.OpenApiRoutePattern = "/openapi/{documentName}.json";
            });
        }
    }

    private static void UseCustomStaticFiles(this IApplicationBuilder app)
    {
        var settings = app.ApplicationServices.GetRequiredService<IOptions<StaticFilesSettings>>().Value;

        if (string.IsNullOrWhiteSpace(settings.StaticFilesPath))
        {
            throw new InvalidOperationException("La ruta 'Storage:StaticFilesPath' no está configurada.");
        }

        if (!Directory.Exists(settings.StaticFilesPath))
        {
            Directory.CreateDirectory(settings.StaticFilesPath);
        }

        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(settings.StaticFilesPath),
            RequestPath = new PathString("/StaticFiles"),
            ServeUnknownFileTypes = true
        });
    }
}