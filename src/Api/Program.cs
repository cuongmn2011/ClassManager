using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Application.Interfaces;
using Api.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using DotNetEnv;
using Infrastructure.Data.Seed;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Infrastructure.Services;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Infrastructure.Security.Authorization;
using Domain.Constants;

Env.Load();
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddIdentity<Domain.Entities.User, Domain.Entities.Role>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Add the Mock Service to the DI container.
builder.Services.AddScoped<IStudentService, MockStudentService>();
builder.Services.AddScoped<ITeacherService, MockTeacherService>();
builder.Services.AddScoped<IBranchService, MockBranchService>();
builder.Services.AddScoped<IClassService, MockClassService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();
//Configure JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JWT_ISSUER"],
        ValidAudience = builder.Configuration["JWT_AUDIENCE"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT_SECRET"]))
    };
});

//Configure Authorization
builder.Services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
builder.Services.AddAuthorization(options =>
{
    // Dynamically add a policy for each permission defined in the Permissions class
    foreach (var permission in Permissions.GetAllPermissions())
    {
        options.AddPolicy(permission, policy =>
            policy.AddRequirements(new PermissionRequirement(permission)));
    }
});

// Add services for controllers.
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "ClassManager API", Version = "v1", Description = "API for ClassManager Application" });
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme { In = Microsoft.OpenApi.Models.ParameterLocation.Header, Description = "Please enter a valid token", Name = "Authorization", Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http, BearerFormat = "JWT", Scheme = "Bearer" });
    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement { { new Microsoft.OpenApi.Models.OpenApiSecurityScheme { Reference = new Microsoft.OpenApi.Models.OpenApiReference { Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme, Id = "Bearer" } }, new string[] { } } });
});

var app = builder.Build();

// This section will automatically apply pending migrations on startup.
// This is convenient for development but should be used with caution in production.
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        // Ensure the database is created and apply any pending migrations
        context.Database.Migrate();

        var userManager = services.GetRequiredService<UserManager<User>>();
        var roleManager = services.GetRequiredService<RoleManager<Role>>();
        // Seed default roles and the admin user
        await ApplicationDbContextSeed.SeedDefaultUserAndRolesAsync(userManager, roleManager);
        // Seed permissions and grant all to admin
        await ApplicationDbContextSeed.SeedPermissionsAsync(context, roleManager);
    }
    catch (Exception ex)
    {
        // Log the error if migration fails
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database.");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Authentication middleware
app.UseAuthentication();
app.UseAuthorization();

// Add middleware for routing to controllers.
app.MapControllers();

app.Run();