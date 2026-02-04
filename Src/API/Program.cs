using API.Extensions;
using API.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServices(builder.Configuration);

var app = builder.Build();


app.ConfigurePipeline();

app.Run();
