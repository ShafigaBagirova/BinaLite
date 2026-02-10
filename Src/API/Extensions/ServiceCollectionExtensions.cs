using API.Options;
using Application.Abstracts.Repositories;
using Application.Abstracts.Services;
using Application.Options;
using Application.Validations.AuthValidation;
using Application.Validations.CityValidation;
using Application.Validations.DistrictValidation;
using Application.Validations.PropertyAdValidation;
using Domain.Entities;
using FluentValidation;
using FluentValidation;
using Infrastructure.Extensions;
using Infrastructure.Extensions;
using Infrastructure.Services;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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

        // Minio/S3 - YALNIZ 1 dəfə
        services.AddMinioStorage(configuration);
        services.AddScoped<IFileStorageService, S3MinioFileStorageService>();

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c => c.CustomSchemaIds(t => t.FullName));

        // Validators (istəsən hamısını saxla)
        services.AddValidatorsFromAssemblyContaining<CreatePropertyAdValidator>();
        services.AddValidatorsFromAssemblyContaining<UpdatePropertyAdValidator>();
        services.AddValidatorsFromAssemblyContaining<CreateCityValidator>();
        services.AddValidatorsFromAssemblyContaining<UpdateCityValidator>();
        services.AddValidatorsFromAssemblyContaining<CreateDistrictValidator>();
        services.AddValidatorsFromAssemblyContaining<UpdateDistrictValidator>();
        services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>(); // auth validatorlar

        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

        // Repos
        services.AddScoped<IPropertyAdRepository, PropertyAdRepository>();
        services.AddScoped<ICityRepository, CityRepository>();
        services.AddScoped<IDistrictRepository, DistrictRepository>();
        services.AddScoped<IPropertyMediaRepository, PropertyMediaRepository>();

        // Services
        services.AddScoped<IPropertyAdService, PropertyAdService>();
        services.AddScoped<ICityService, CityService>();
        services.AddScoped<IDistrictService, DistrictService>();
        services.AddScoped<IFileService, FileService>();

        // ✅ Identity - YALNIZ 1 dəfə
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

        // Jwt options
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));

        // ✅ Authentication + JwtBearer (default scheme-i zorla Bearer etmə)
        services.AddAuthentication()
            .AddJwtBearer();

        services.ConfigureOptions<ConfigureJwtBearerOptions>();

        // Auth
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IAuthService, AuthService>();

        return services;
    }
}
