using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using System.Data;
using EcomAPI.Interfaces;
using EcomAPI.Services;

// TO DO LIST:
// 1. HASH THE PASSWORDS STORED IN DB
// 2. CREATE A NEW SERVICE THAT HANDLES CREATING AND CONFIGURING A NEW TOKEN
// 3. ADD LOGIN ENDPOINT TO VALIDATE USER AND RETURN JWT TOKEN

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
