using EcomAPI;
using EcomAPI.Interfaces;
using EcomAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System.Data;


// TO DO LIST:
// 1. START WORKING ON PRODUCT APIS (CRUD)
// 2. IMPLEMENT ORDERS CONTROLLER
// 3. ADD ENDPOINT TO UPDATE USER DETAILS
// 4. ADD OTP SERVICE TO START USING IT IN MULTIPLE PLACES
// 5. MAKE FILE CREATION AS A UTILITY CLASS TO AVOID REPETITION
// 6. APPLY IFormFile instead of statis urls for Products as well

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("fixed", opt =>
    {
        opt.Window = TimeSpan.FromMinutes(1);
        opt.PermitLimit = 10;
        opt.QueueLimit = 0;
    });

    options.RejectionStatusCode = 429;
});

// Add services to the container.
builder.Services.AddControllers().ConfigureApiBehaviorOptions(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

builder.Services.AddScoped<IDbConnection>(sp => new SqlConnection(builder.Configuration.GetConnectionString("DefaultSQLConnection")));
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

app.UseExceptionHandler(appError =>
{
    appError.Run(async context =>
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";

        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        var ex = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>().Error;

        logger.LogError(ex,
            "Unhandled exception on {Method} {Path}",
            context.Request.Method,
            context.Request.Path);

        var response = new EcomAPI.Responses.ApiResponse
        {
            Status = 500,
            Message = "An unexpected error occurred"
        };

        await context.Response.WriteAsJsonAsync(response);
    });
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.UseRateLimiter();

app.MapControllers();

app.Run();