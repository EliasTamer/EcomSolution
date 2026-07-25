using Azure.Storage.Blobs;
using EcomAPI;
using EcomAPI.Interfaces;
using EcomAPI.Services;
using EcomAPI.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.Text.Json.Serialization;


// TO DO LIST:
// 1. CREATE AZURE STORAGE AND START SAVING DATA THERE 
// 2. STORE USER SESSIONS IN REDIS

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
// map strings to enums where valid
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.SuppressModelStateInvalidFilter = true;
    })
    .AddJsonOptions(o =>
        o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddScoped<IDbConnection>(sp => new SqlConnection(builder.Configuration.GetConnectionString("DefaultSQLConnection")));
builder.Services.AddScoped<IUsersService, UsersService>();
builder.Services.AddScoped<IProductCategoriesService, ProductCategoriesService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IOrdersService, OrdersService>();
builder.Services.AddScoped<IProductsService, ProductsService>();
builder.Services.AddSingleton(_ => new BlobServiceClient(builder.Configuration.GetConnectionString("AzureStorage")));


builder.Services.AddKeyedSingleton<IFileService>(FileStores.UserPhotos, (sp, key) => new FileService(sp.GetRequiredService<BlobServiceClient>(), "user-photos", [".jpg", ".png"], 5));
builder.Services.AddKeyedSingleton<IFileService>(FileStores.ProductCategory, (sp, key) => new FileService(sp.GetRequiredService<BlobServiceClient>(), "product-category-photos", [".jpg", ".png"], 5));
builder.Services.AddKeyedSingleton<IFileService>(FileStores.ProductPhotos, (sp, key) => new FileService(sp.GetRequiredService<BlobServiceClient>(), "product-photos", [".jpg", ".png"], 5));


builder.Services.AddAutoMapper(typeof(MappingConfig));

var jwtSecret = builder.Configuration["JwtSettings:Secret"] ?? throw new InvalidOperationException("JwtSettings:Secret is not configured.");
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
builder.Services.AddApplicationInsightsTelemetry();

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


app.UseHttpsRedirection();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();