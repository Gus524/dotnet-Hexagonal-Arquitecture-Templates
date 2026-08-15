using FileStorage.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Configuration;
using Testcontainers.MsSql;

namespace IntegrationTests.Shared;

public class WebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly MsSqlContainer _msSqlContainer = new MsSqlBuilder("mcr.microsoft.com/mssql/server:2022-latest")
        .Build();
    
    private readonly string _tempStoragePath;
    private readonly string _tempStaticFilesPath;

    public WebAppFactory()
    {
        _tempStoragePath = Path.Combine(Path.GetTempPath(), $"IntegrationTests_Storage_{Guid.NewGuid()}");
        _tempStaticFilesPath = Path.Combine(Path.GetTempPath(), $"IntegrationTests_Static_{Guid.NewGuid()}");
        
        Directory.CreateDirectory(_tempStoragePath);
        Directory.CreateDirectory(_tempStaticFilesPath);
    }

    public async Task InitializeAsync()
    {
        await _msSqlContainer.StartAsync();
    }
    
    public new async Task DisposeAsync()
    {
        await _msSqlContainer.DisposeAsync();
        await base.DisposeAsync();
        
        CleanupTempDirectories();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        
        var containerConnectionString = _msSqlContainer.GetConnectionString();
        
        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                // 1. Cadenas de conexión por tenant — BDs independientes dentro del mismo contenedor SQL Server.
                //    ConnectionResolver lee ConnectionStrings:{tenantId} para resolver IdentityContext por tenant.
                { "ConnectionStrings:Tenant1", WithDatabase(containerConnectionString, "Tenant1_IntegrationDb") },
                { "ConnectionStrings:Tenant2", WithDatabase(containerConnectionString, "Tenant2_IntegrationDb") },
                { "ConnectionStrings:Tenant3", WithDatabase(containerConnectionString, "Tenant3_IntegrationDb") },

                // 2. Configuración JWT
                { "JWTSettings:JWT_Secret",          "EstaEsUnaClaveSecretaMuyLargaParaPruebas123!" },
                { "JWTSettings:JWT_ISSUER_TOKEN",    "TestIssuer" },
                { "JWTSettings:JWT_AUDIENCE_TOKEN",  "TestAudience" },
                { "JWTSettings:ExpiryInMinutes",     "60" },

                // 3. Configuración FileStorage
                { $"{FileStorageSettings.SectionName}:BasePath", _tempStoragePath },
                
                // 4. Configuración StaticFiles
                { $"{StaticFilesSettings.SectionName}:StaticFilesPath", _tempStaticFilesPath },
            });
        });
 
        builder.ConfigureTestServices(services =>
        {
            var outboxService = services.FirstOrDefault(d => d.ImplementationType?.Name == "MultitenantOutBoxProcessor");
            if (outboxService != null) services.Remove(outboxService);
        });
    }

    /// <summary>
    /// Expone la cadena de conexión del contenedor para pruebas de integración que
    /// operan directamente sobre IdentityContext (sin pasar por el host WebAPI).
    /// Garantiza que las pruebas usen las mismas BDs aisladas por tenant que la WebAppFactory.
    /// </summary>
    public string GetTenantConnectionString(string tenantKey) =>
        WithDatabase(_msSqlContainer.GetConnectionString(), tenantKey switch
        {
            "Tenant1" => "Tenant1_IntegrationDb",
            "Tenant2" => "Tenant2_IntegrationDb",
            "Tenant3" => "Tenant3_IntegrationDb",
            _         => $"{tenantKey}_IntegrationDb"
        });

    /// <summary>Construye una cadena de conexión apuntando a una base de datos específica en el contenedor.</summary>
    private static string WithDatabase(string connectionString, string database) =>
        new Microsoft.Data.SqlClient.SqlConnectionStringBuilder(connectionString)
        {
            InitialCatalog = database
        }.ConnectionString;
    
    private static void RemoveDbContext<T>(IServiceCollection services) where T : DbContext
    {
        var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<T>));
        if (descriptor is not null) services.Remove(descriptor);
        
        var contextDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(T));
        if (contextDescriptor is not null) services.Remove(contextDescriptor);
    }
    
    /// <summary>Obtiene la ruta base de almacenamiento temporal para verificar archivos guardados</summary>
    public string TempStoragePath => _tempStoragePath;

    /// <summary>Obtiene la ruta de archivos estáticos temporales</summary>
    public string TempStaticFilesPath => _tempStaticFilesPath;

    private void CleanupTempDirectories()
    {
        if (Directory.Exists(_tempStoragePath))
            try { Directory.Delete(_tempStoragePath, true); } catch { /* Ignorar errores de limpieza */ }
        
        if (Directory.Exists(_tempStaticFilesPath))
            try { Directory.Delete(_tempStaticFilesPath, true); } catch { /* Ignorar errores de limpieza */ }
    }
}
