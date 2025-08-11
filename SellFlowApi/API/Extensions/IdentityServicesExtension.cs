using System;
using System.Text;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Google.Apis.Auth.AspNetCore3;
using API.Helpers;
using API.interfaces;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
namespace API.Extensions;

public static class IdentityServiceExtensions
{
    public static IServiceCollection AddIdentityServices(this IServiceCollection services,
    IConfiguration config)
    {
        services.AddIdentity<AppUser, AppRole>(opt =>
        {
            opt.Password.RequireNonAlphanumeric = false;
        })
        .AddRoles<AppRole>()
        .AddRoleManager<RoleManager<AppRole>>()
        .AddEntityFrameworkStores<DataContext>()
        .AddDefaultTokenProviders();

        services.AddAuthentication(options => 
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(Options =>
        {
            var tokenKey = config["TokenKey"]
                ?? throw new Exception("TokenKey not found");
            Options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey)),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                RoleClaimType = "role",
                NameClaimType = ClaimTypes.Name,
                ClockSkew = TimeSpan.Zero
            };
            
            Options.Events = new JwtBearerEvents
            {
                OnTokenValidated = context =>
                {
                    var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<JwtBearerHandler>>();
                    
                    // Log all claims for debugging
                    var claims = context.Principal.Claims;
                    logger.LogInformation("Token validated. Claims: {Claims}", 
                        string.Join(", ", claims.Select(c => $"{c.Type}: {c.Value}")));

                    // Ensure role claims are properly transformed
                    var roleClaims = claims.Where(c => c.Type == "role").ToList();
                    if (roleClaims.Any())
                    {
                        var identity = context.Principal.Identity as ClaimsIdentity;
                        if (identity != null)
                        {
                            // Remove existing role claims
                            var existingRoleClaims = identity.Claims.Where(c => c.Type == ClaimTypes.Role).ToList();
                            foreach (var claim in existingRoleClaims)
                            {
                                identity.RemoveClaim(claim);
                            }

                            // Add role claims with the correct type
                            foreach (var roleClaim in roleClaims)
                            {
                                identity.AddClaim(new Claim(ClaimTypes.Role, roleClaim.Value));
                            }
                        }
                    }

                    return Task.CompletedTask;
                },
                OnAuthenticationFailed = context =>
                {
                    var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<JwtBearerHandler>>();
                    logger.LogError("Authentication failed: {Error}", context.Exception);
                    return Task.CompletedTask;
                }
            };
        });

        services.AddAuthorization(options =>
        {
            options.AddPolicy("RequireAdminRole", policy => 
            {
                policy.RequireAuthenticatedUser();
                policy.RequireAssertion(context =>
                {
                    var logger = context.Resource as ILogger<Program>;
                    var user = context.User;
                    
                    // Check for Admin role using direct claim check
                    var hasAdminRole = user.Claims.Any(c => 
                        c.Type == ClaimTypes.Role && 
                        c.Value.Equals("Admin", StringComparison.OrdinalIgnoreCase));
                    
                    if (logger != null)
                    {
                        logger.LogInformation("Policy evaluation - Has Admin role: {HasAdminRole}", hasAdminRole);
                    }
                    return hasAdminRole;
                });
            });

            options.AddPolicy("RequireModeraterRole", policy => 
            {
                policy.RequireAuthenticatedUser();
                policy.RequireAssertion(context =>
                {
                    var user = context.User;
                    return user.Claims.Any(c => 
                        c.Type == ClaimTypes.Role && 
                        (c.Value.Equals("Admin", StringComparison.OrdinalIgnoreCase) || 
                         c.Value.Equals("Moderator", StringComparison.OrdinalIgnoreCase)));
                });
            });
        });

        // Register the GoogleService for dependency injection
        return services;
    }
}
