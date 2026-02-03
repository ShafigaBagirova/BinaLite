using API.Middlewares;
using Application.Abstracts.Repositories;
using Application.Abstracts.Services;
using Application.Mappings;
using Application.Validations.CityValidation;
using Application.Validations.DistrictValidation;
using Application.Validations.PropertyAdValidation;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;
using Persistence.Repositories;
using Persistence.Services;
using System;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });
    

builder.Services.AddValidatorsFromAssemblyContaining<CreatePropertyAdValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UpdatePropertyAdValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateCityValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UpdateCityValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateDistrictValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UpdateDistrictValidator>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddDbContext<BinaLiteDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IPropertyAdRepository, PropertyAdRepository>();
builder.Services.AddScoped<IPropertyAdService, PropertyAdService>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddScoped<ICityRepository, CityRepository>();
builder.Services.AddScoped<ICityService, CityService>();
builder.Services.AddScoped<IDistrictRepository, DistrictRepository>();
builder.Services.AddScoped<IDistrictService, DistrictService>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.CustomSchemaIds(t => t.FullName);
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseMiddleware<ExceptionMiddleware>();


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
