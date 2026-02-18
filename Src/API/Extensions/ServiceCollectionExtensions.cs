using API.Options;
using Application.Abstracts.Repositories;
using Application.Abstracts.Services;
using Application.Options;
using Application.Validations.AuthValidation;
using Application.Validations.CityValidation;
using Application.Validations.DistrictValidation;
using Application.Validations.PropertyAdValidation;
using Domain.Constants;
using Domain.Entities;
using FluentValidation;
using Infrastructure.Extensions;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Persistence.Context;
using Persistence.Repositories;
using Persistence.Services;


namespace API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();

        services.AddDbContext<BinaLiteDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

       
        services.AddMinioStorage(configuration);
        services.AddScoped<IFileStorageService, S3MinioFileStorageService>();

        services.AddEndpointsApiExplorer();
        
        services.AddSwaggerGen(c =>
        {
           
            
            c.CustomSchemaIds(t => t.FullName);

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "JWT Authorization header. Example: \"Bearer {token}\""
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            Array.Empty<string>()
        }
    });
        });


        services.AddValidatorsFromAssemblyContaining<CreatePropertyAdValidator>();
        services.AddValidatorsFromAssemblyContaining<UpdatePropertyAdValidator>();
        services.AddValidatorsFromAssemblyContaining<CreateCityValidator>();
        services.AddValidatorsFromAssemblyContaining<UpdateCityValidator>();
        services.AddValidatorsFromAssemblyContaining<CreateDistrictValidator>();
        services.AddValidatorsFromAssemblyContaining<UpdateDistrictValidator>();
        services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>(); 

        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

        services.AddScoped<IPropertyAdRepository, PropertyAdRepository>();
        services.AddScoped<ICityRepository, CityRepository>();
        services.AddScoped<IDistrictRepository, DistrictRepository>();
        services.AddScoped<IPropertyMediaRepository, PropertyMediaRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        


        services.AddScoped<IPropertyAdService, PropertyAdService>();
        services.AddScoped<ICityService, CityService>();
        services.AddScoped<IDistrictService, DistrictService>();
        services.AddScoped<IFileService, FileService>();
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IEmailService, SmtpEmailService>();

        services
            .AddIdentity<User, IdentityRole>(opt =>
            {
                opt.User.RequireUniqueEmail = true;

                opt.Password.RequiredLength = 8;
                opt.Password.RequireDigit = true;
                opt.Password.RequireLowercase = true;
                opt.Password.RequireUppercase = true;
                opt.Password.RequireNonAlphanumeric = true;
            })
            .AddEntityFrameworkStores<BinaLiteDbContext>()
            .AddDefaultTokenProviders();

        services.AddAuthorization();


        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
      
        var jwtSection = configuration.GetSection(JwtOptions.SectionName);
        if (!jwtSection.Exists())
            throw new InvalidOperationException(
                $"Configuration section '{JwtOptions.SectionName}' is missing.");

        var jwtOptions = jwtSection.Get<JwtOptions>()
            ?? throw new InvalidOperationException("JwtOptions could not be bound from configuration.");

        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer();
        services.ConfigureApplicationCookie(options =>
        {
            options.Events = new CookieAuthenticationEvents
            {
                OnRedirectToLogin = context =>
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return Task.CompletedTask;
                },
                OnRedirectToAccessDenied = context =>
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    return Task.CompletedTask;
                }
            };
        });
        services.AddAuthorization(options =>
        {
            options.AddPolicy(Policies.ManageCities, policy =>
                policy.RequireRole(RoleNames.Admin));

            options.AddPolicy(Policies.ManageProperties, policy =>
                policy.RequireAuthenticatedUser());
            options.AddPolicy(Policies.ModerateProperties, p => p.RequireRole(RoleNames.Admin));
        });


        services.ConfigureOptions<ConfigureJwtBearerOptions>();
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.Configure<SeedOptions>(configuration.GetSection(SeedOptions.SectionName));
        services.Configure<EmailOptions>(configuration.GetSection(EmailOptions.SectionName));
        services.AddHttpContextAccessor();

        services.AddScoped<IPropertyAdService, PropertyAdService>();
        services.AddScoped<IPropertyModerationService, PropertyModerationService>();

        return services;
    }
}
