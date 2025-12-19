using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using System.Data;
using EcomAPI.Interfaces;
using EcomAPI.Services;

// TO DO LIST:
// 1. ERROR HANDLING FOR CreateUser IN CONTROLLER
// 2. CREATE A GENERIC RESPONSE CLASS FOR ALL ENDPOINTS
// 3. HASH THE PASSWORDS STORED IN DB
// 4. CAST STATUS PROPERTY IN ApiResponse TO INT

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddScoped<IDbConnection>(sp =>  new SqlConnection(builder.Configuration.GetConnectionString("DefaultSQLConnection")));
builder.Services.AddScoped<IUsersService, UsersService>();

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
