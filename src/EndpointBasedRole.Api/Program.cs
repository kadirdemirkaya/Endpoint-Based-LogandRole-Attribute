using EndpointBasedRole;
using EndpointBasedRole.Api.Filters;
using EndpointBasedRole.Extensions;
using EndpointBasedRole.Filters;
using EndpointBasedRole.Models.Entity;
using EndpointBasedRole.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(opt =>
{
    opt.Filters.Add<RolePermissionFilter>();
});

builder.Services.AddScoped<SpecificLogFilter>();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Endpoint_Attribute_App",
        Version = "V1",
        Description = "",
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] { }
                    }
                });
});

var sqlServerOptions = builder.Services.GetOptions<SqlServerOptions>("SqlServerOptions");

builder.Services.AddDbContext<EndpointBaseRoleDbContext>(options =>
{
    options.UseSqlServer(
        sqlServerOptions.SqlConnection,
        sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: int.Parse(sqlServerOptions.RetryCount),
                maxRetryDelay: TimeSpan.FromSeconds(int.Parse(sqlServerOptions.RetryDelay)),
                errorNumbersToAdd: null);
        });
});

builder.Services.AddIdentity<User, Role>(options =>
{
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 3;
}).AddEntityFrameworkStores<EndpointBaseRoleDbContext>()
            .AddDefaultTokenProviders();

builder.Services.EndpointBasedRoleServiceRegistration();

var sp = builder.Services.BuildServiceProvider();

EndpointBaseRoleSeedContext.SeedAsync(sp.GetRequiredService<EndpointBaseRoleDbContext>()).GetAwaiter().GetResult();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("User", policy =>
    {
        policy.RequireClaim(ClaimTypes.Role, "User");
    });
    options.AddPolicy("UserOrAdmin", policy =>
             policy.RequireAssertion(context =>
                 context.User.HasClaim(ClaimTypes.Role, "Admin") ||
                 context.User.HasClaim(ClaimTypes.Role, "User")));
});


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Endpoint_Attribute_App");
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
