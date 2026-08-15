using Identity.Contexts;
using Identity.Data;
using Identity.Initializer;
using Identity.Services;
using Identity.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using Core.IAM.Application.Features.Auth.Common.Ports;
using IAM.Application.Features.Users.Common.Ports;
using SharedKernel.Ports.Out.MultiTenancy;
using SharedKernel.Ports.Out.Repository;

namespace Identity;

public static class ServiceExtensions
{
    public static void AddIdentityInfraestructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IdentityAdapter>();
        services.AddScoped<IUserAuthenticator>(sp => sp.GetRequiredService<IdentityAdapter>());
        services.AddScoped<ISessionRefresher>(sp => sp.GetRequiredService<IdentityAdapter>());
        services.AddScoped<IUserProfileQuery>(sp => sp.GetRequiredService<IdentityAdapter>());
        services.AddScoped<IAuthPort>(sp => sp.GetRequiredService<IdentityAdapter>());
        
        services.AddScoped<UserIdentityRepository>();

        services.AddScoped<IUserIdentityRepository>(sp => 
            sp.GetRequiredService<UserIdentityRepository>());
        
        services.AddScoped<IReadIdentityRepository>(sp => 
            sp.GetRequiredService<UserIdentityRepository>());
        
        services.AddScoped<IWriteIdentityRepository>(sp => 
            sp.GetRequiredService<UserIdentityRepository>());
        
        services.AddScoped<IDbContextInitializer, IdentityInitializer>();
        services.AddScoped<IConnectionResolver, ConnectionResolver>();
        services.AddDbContext<IdentityContext>();

        services.AddIdentityCore<ApplicationUser>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 4;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredUniqueChars = 0;

                options.User.AllowedUserNameCharacters =
                    "abcdefhgijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";

                options.User.RequireUniqueEmail = false;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<IdentityContext>()
            .AddDefaultTokenProviders();

        services.Configure<JwtSettings>(configuration.GetSection("JWTSettings"));
        
        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(o =>
            {
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidIssuer = configuration["JWTSettings:JWT_ISSUER_TOKEN"],
                    ValidAudience = configuration["JWTSettings:JWT_AUDIENCE_TOKEN"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                        configuration["JWTSettings:JWT_Secret"] ??
                        throw new InvalidOperationException("JWT Key not found"))),
                    RoleClaimType = ClaimTypes.Role,
                    NameClaimType = ClaimTypes.Name
                };
            });
    }
}