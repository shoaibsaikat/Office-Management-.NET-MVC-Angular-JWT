using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

using Office_Management_.NET_MVC_Angular_JWT.Models;
using Office_Management_.NET_MVC_Angular_JWT.Utils;
using Office_Management_.NET_MVC_Angular_JWT.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();

// MariaDB
var connectionString = builder.Configuration.GetConnectionString("MariaDbConnectionString");
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

// CORS
var  OfficeManagementOrigins = "_officeManagementSpecificOrigins";
// TODO: allow specific origin does not work
// builder.Services.AddCors(options =>
// {
//     options.AddPolicy(name: OfficeManagementOrigins, builder =>
//     {
//         builder.WithOrigins("http://localhost:44427")
//                 .WithMethods("PUT", "POST", "GET")
//                 .WithHeaders("Content-Type", "Authorization");
//     });
// });

// allow all
builder.Services.AddCors(options => options.AddPolicy(OfficeManagementOrigins, builder =>
{
    builder.WithOrigins("*").WithMethods("PUT", "POST", "GET").WithHeaders("Content-Type", "Authorization");
}));

// JWT
var jwtKey = builder.Configuration.GetValue<string>("JwtKey");
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
            ValidIssuer = builder.Configuration.GetValue<string>("JwtIssuer"),
            ValidAudience = builder.Configuration.GetValue<string>("JwtAudience"),
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

app.UseRouting();
app.UseCors(OfficeManagementOrigins);
app.UseHttpsRedirection();
app.UseStaticFiles();

//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller}/{action=Index}/{id?}");

app.UseAuthorization();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();

// TODO: need to fix appsettings keys not read issue and fix CORS issue
