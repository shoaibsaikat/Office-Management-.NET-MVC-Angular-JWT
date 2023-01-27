using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

using Office_Management_.NET_MVC_Angular_JWT.Models;
using Office_Management_.NET_MVC_Angular_JWT.Utils;
using Office_Management_.NET_MVC_Angular_JWT.Repositories;
using Microsoft.Extensions.Configuration;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();

// MariaDB
//var connectionString = builder.Configuration.GetConnectionString("MariaDbConnectionString");
var connectionString = "server=localhost;port=3306;user=root;password=;database=inventory_dotnet_test;AllowZeroDateTime=true;";
builder.Services.AddDbContext<ApplicationDbContext>(dbContextOptions => dbContextOptions
        .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
        // The following three options help with debugging, but should
        // be changed or removed for production.
        .LogTo(Console.WriteLine, LogLevel.Information)
        .EnableSensitiveDataLogging()
        .EnableDetailedErrors()
);

// Repositories
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IAssetRepository, AssetRepository>();
builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();
builder.Services.AddScoped<IRequisitionRepository, RequisitionRepository>();
builder.Services.AddScoped<ILeaveRepository, LeaveRepository>();
builder.Services.AddScoped<ITokenUtil, JWTTokenUtil>();
builder.Services.AddScoped<IAccountUtil, AccountUtil>();
builder.Services.AddScoped<ICommonUtil, CommonUtil>();

// JWT
//var jwtKey = builder.Configuration.GetValue<string>("JwtKey");
var jwtKey = "BprQAVEPuh9U31ruRa70Z2cwqiyzOsavobrkpD6lXvy62w4p4P8ZLGUSAsWFvwTdCysc2NeJX0R3FbNhrZVPmVVVRIim62TLeLlKbxlj0nN9rZ9vTmZ9AEk9";
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear(); // => remove default claims
if (jwtKey != null)
{
    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(cfg =>
    {
        cfg.RequireHttpsMetadata = false;
        cfg.SaveToken = true;
        cfg.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = "https://localhost:7258;",//builder.Configuration.GetValue<string>("JwtIssuer"),
            ValidAudience = "https://localhost:44427;",//builder.Configuration.GetValue<string>("JwtAudience"),
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey == null ? "" : jwtKey)),
            ClockSkew = TimeSpan.Zero // remove delay of token when expire
        };
    });
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();


//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller}/{action=Index}/{id?}");

app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();

// TODO: need to fix appsettings keys not read issue