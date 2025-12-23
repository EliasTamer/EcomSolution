using Microsoft.Data.SqlClient;
using System.Data;
using EcomAPI.Interfaces;
using EcomAPI.Services;

// TO DO LIST:
// 1. ADD CHANGE PASSWORD ENDPOINT
// 2. CREATE PRODUCTS, PRODUCT CATEGORIES AND ORDERS TABLES AND THEIR RESPECTIVE ENDPOINTS
// 3. ADD GET USER PROFILE ENDPOINT
// 4. ADD FILERING AND PAGINATION TO PRODUCTS ENDPOINT
// 5. PROTECT ENDPOINTS USING JWT AUTHENTICATION

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers().ConfigureApiBehaviorOptions(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

builder.Services.AddScoped<IDbConnection>(sp =>  new SqlConnection(builder.Configuration.GetConnectionString("DefaultSQLConnection")));
builder.Services.AddScoped<IUsersService, UsersService>();
builder.Services.AddScoped<IJwtService, JwtService>();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
