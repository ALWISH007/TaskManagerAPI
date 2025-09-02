using Microsoft.AspNetCore.Mvc;// brings ControllerBase, NotFound(), Ok()
using TaskManagerAPI.Models;
using TaskManagerAPI.Services;
using Microsoft.EntityFrameworkCore;
using TaskManagerAPI.Data;
using TaskManagerAPI.Services.Auth;
using TaskManagerAPI.Models.Auth;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
//using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;


var builder = WebApplication.CreateBuilder(args);


// Get the frontend URL from an environment variable (used in production) or fall back to development settings
var frontendUrl = Environment.GetEnvironmentVariable("FRONTEND_URL") ?? builder.Configuration["FrontendUrl"] ?? "http://localhost:5500";

// Add CORS policy to allow our frontend to call this API
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowMyFrontend",
        policy =>
        {
            policy.WithOrigins(frontendUrl, "null")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

// Use a JWT Key from an environment variable (MANDATORY for production) or fall back to the hardcoded one (for development only)
var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY") ?? builder.Configuration["Jwt:Key"];

// Configure JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey!))
        };
    });

/*
        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("MustBeOver18", policy =>
                policy.RequireClaim("Age", "18"));
        });

*/

// Add services to the container.
// This line is CRUCIAL - it registers your controllers
builder.Services.AddControllers();//Add all the services required for MVC Controllers (but not Razor Pages or Views)
//routing, model binding, JSON, validation, etc.


// Use a connection string from an environment variable (common in production clouds) or fall back to appsettings.json
var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING") ?? builder.Configuration.GetConnectionString("DefaultConnection");

// Register the DbContext. We'll use SQLite and store the database in a file called "app.db"
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(connectionString));


builder.Services.AddScoped<IAuthService, SimpleAuthService>();
builder.Services.AddScoped<ITaskService, TaskService>();

var app = builder.Build();

app.UseStaticFiles();

app.UseCors("AllowMyFrontend"); // Use the CORS policy

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{

//}

//app.UseHttpsRedirection(); //forces HTTPS.

app.UseAuthentication(); //checks who the user is.

app.UseAuthorization(); //checks what the user is allowed to do.

app.MapControllers(); //routes the request to the right controller.
// This maps your controller endpoints


//For Testing server endpoint
//app.MapGet("/test", () => "Hello World! API is working!");

app.Run();

