using Application.Abstracts.Repositories;
using Application.Abstracts.Services;
using Application.Validations.CityValidation;
using Application.Validations.DistrictValidation;
using Application.Validations.PropertyAdValidation;
using Domain.Entities;
using FluentValidation;
using Infrastructure.Services;
using Infrastructure.Extensions;
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
        services.AddMinioStorage(configuration);
        services.AddDbContext<BinaLiteDbContext>(options =>
            options.UseSqlServer(
               configuration.GetConnectionString("DefaultConnection")));
        services
          .AddIdentity<User, IdentityRole>(options =>
          {
              options.Password.RequiredLength = 6;
              options.Password.RequiredUniqueChars = 1;
          })
          .AddEntityFrameworkStores<BinaLiteDbContext>()
          .AddDefaultTokenProviders();

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.CustomSchemaIds(t => t.FullName);
        });
       
        services.AddValidatorsFromAssemblyContaining<CreatePropertyAdValidator>();
        services.AddValidatorsFromAssemblyContaining<UpdatePropertyAdValidator>();
        services.AddValidatorsFromAssemblyContaining<CreateCityValidator>();
        services.AddValidatorsFromAssemblyContaining<UpdateCityValidator>();
        services.AddValidatorsFromAssemblyContaining<CreateDistrictValidator>();
        services.AddValidatorsFromAssemblyContaining<UpdateDistrictValidator>();

        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        
        services.AddScoped<IPropertyAdRepository, PropertyAdRepository>();
        services.AddScoped<ICityRepository, CityRepository>();
        services.AddScoped<IDistrictRepository, DistrictRepository>();
        services.AddScoped<IPropertyMediaRepository, PropertyMediaRepository>();
        
        services.AddScoped<IPropertyAdService, PropertyAdService>();
        services.AddScoped<ICityService, CityService>();
        services.AddScoped<IDistrictService, DistrictService>();
        services.AddScoped<IFileService, FileService>();
        
        services.AddScoped<IFileStorageService, S3MinioFileStorageService>();
        services.AddMinioStorage(configuration);
        services.AddScoped<IFileStorageService, S3MinioFileStorageService>();

        return services;
    }
}
