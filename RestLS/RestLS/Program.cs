using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RestLS.Auth;
using RestLS.Auth.Models;
using RestLS.Data;
using RestLS.Data.Repositories;

var builder = WebApplication.CreateBuilder(args);
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
// ORM - Microsoft.EntityFrameworkCore.SqlServer
// Microsoft.EntityFrameworkCore.Tools
// Orm managment - dotnet tool install --global dotnet-ef
// Microsoft.AspNetCore.Identity
// Microsoft.AspNetCore.Identity.EntityFrameworkCore
// Microsoft.AspNetCore.Authentication.JwtBearer


builder.Services.AddControllers();

builder.Services.AddIdentity<ClinicUser, IdentityRole>()
    .AddEntityFrameworkStores<LS_DbContext>()
    .AddDefaultTokenProviders();



builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters.ValidAudience = builder.Configuration["JWT:ValidAudience"];
        options.TokenValidationParameters.ValidIssuer = builder.Configuration["JWT:ValidIssuer"];
        options.TokenValidationParameters.IssuerSigningKey =
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]));
        options.TokenValidationParameters.ClockSkew = new TimeSpan(0, 0, 5);
    });
    
builder.Services.AddDbContext<LS_DbContext>();
//adding repost
builder.Services.AddTransient<ITherapiesRepository, TherapiesRepository>();
builder.Services.AddTransient<IAppointmentsRepository, AppointmentsRepository>();
builder.Services.AddTransient<IRecomendationsRepository, RecomendationsRepository>();
builder.Services.AddTransient<ITestsRepository, TestsRepository>();
builder.Services.AddTransient<INotesRepository, NotesRepository>();
builder.Services.AddTransient<INotificationsRepository, NotificationsRepository>();
builder.Services.AddTransient<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<AuthDbSeeder>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(PolicyNames.ResourceOwner, policy => policy.Requirements.Add(new ResourceOwnerRequirement()));
});

builder.Services.AddSingleton<IAuthorizationHandler, ResourceOwnerAuthorizationHandler>();

var app = builder.Build();

app.UseRouting();

app.UseCors(options =>
{
    options.WithOrigins("http://localhost:3000") // Specify your frontend URL
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials(); // Enable credentials
});

app.MapControllers();
app.UseAuthentication();
app.UseAuthorization();

using var scope = app.Services.CreateScope();
var dbContext = scope.ServiceProvider.GetRequiredService<LS_DbContext>();
dbContext.Database.Migrate();

var dbSeeder = app.Services.CreateScope().ServiceProvider.GetRequiredService<AuthDbSeeder>();
await dbSeeder.SeedAsync();

app.Run();

//migration after changing entities  
// dotnet ef migrations add 'comment'
// dotnet ef database update