using System;
using System.Text;
using API.Data;
using API.Entities;
using API.interfaces;
using API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using AutoMapper;
using API.Extensions;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Cookies;
using API.DATA;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration);
builder.Services.AddScoped<IUniProgramRepository, UniProgramRepository>();
builder.Services.AddScoped<IApplicationRepository, ApplicationRepository>();
// ======== MAILERSEND SMTP CONFIGURATION ========
// 1. Bind SMTP settings from configuration
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));

// 2. Register SMTP email sender service
builder.Services.AddTransient<IEmailSender, SmtpEmailSender>();

builder.Services.AddAuthentication()
    .AddCookie()
    .AddGoogle(googleOptions =>
    {
        googleOptions.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme; 
        googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"];
        googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
        //googleOptions.Events.OnTicketReceived = context =>
        //{
        //    context.Response.Redirect("https://localhost:4200");
        //    context.HandleResponse();
        //    return Task.CompletedTask;
        //};
    });

var app = builder.Build();


//added


/*******************************/
app.UseCors(x => x
    .AllowAnyHeader()
    .AllowAnyMethod()
    .WithOrigins("http://localhost:4200", "https://localhost:4200")
    .AllowCredentials()
    );
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();

// Program.cs
app.UseRouting();
app.UseCookiePolicy(new CookiePolicyOptions
{
    MinimumSameSitePolicy = SameSiteMode.None,
    Secure = CookieSecurePolicy.Always
});
app.UseAuthentication(); // Make sure this comes before UseAuthorization
app.UseAuthorization();
app.MapControllers(); // Or app.UseEndpoints(endpoints => endpoints.MapControllers());

app.UseDefaultFiles();
app.UseStaticFiles();

//
using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
try
{
    var context = services.GetRequiredService<DataContext>();
    var userManager = services.GetRequiredService<UserManager<AppUser>>();
    var roleManager = services.GetRequiredService<RoleManager<AppRole>>();
    await context.Database.MigrateAsync();


    // Seed roles if they don't exist
    //var roles = new[] { "Admin", "Moderator" };
    //foreach (var role in roles)
    //{
    //    if (!await roleManager.RoleExistsAsync(role))
    //    {
    //        await roleManager.CreateAsync(new AppRole { Name = role });
    //    }
    //}
}
catch (Exception ex)
{
    var logger = services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error occurred during migration");
}
//
app.Run();
