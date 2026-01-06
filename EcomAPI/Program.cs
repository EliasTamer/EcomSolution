using EcomAPI;
using Microsoft.Data.SqlClient;
using System.Data;
using EcomAPI.Interfaces;
using EcomAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;


// TO DO LIST:
// 1. CREATE PRODUCTS, PRODUCT CATEGORIES AND ORDERS TABLES AND THEIR RESPECTIVE ENDPOINTS
// 2. ADD FILERING AND PAGINATION TO PRODUCTS ENDPOINT

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers().ConfigureApiBehaviorOptions(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

builder.Services.AddScoped<IDbConnection>(sp =>  new SqlConnection(builder.Configuration.GetConnectionString("DefaultSQLConnection")));
builder.Services.AddScoped<IUsersService, UsersService>();
builder.Services.AddScoped<IProductCategoriesService, ProductCategoriesService>();
builder.Services.AddScoped<IJwtService, JwtService>();

builder.Services.AddAutoMapper(typeof(MappingConfig));

var jwtSecret = builder.Configuration["JwtSettings:Secret"];
var jwtIssuer = builder.Configuration["JwtSettings:Issuer"];
var jwtAudience = builder.Configuration["JwtSettings:Audience"];

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwtSecret))
    };
});

builder.Services.AddAuthorization();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
