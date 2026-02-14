using API.Middlewares;
using Application.Options;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Persistence.Data;

namespace API.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        ConfigurePipelineAsync(app).GetAwaiter().GetResult();
        return app;
    }
    public  static async Task<WebApplication> ConfigurePipelineAsync(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var sp = scope.ServiceProvider;

            await RoleSeeder.SeedAsync(
                sp.GetRequiredService<RoleManager<IdentityRole>>());

            if (app.Environment.IsDevelopment())
            {
                var adminSeeder = new AdminSeeder(
                    sp.GetRequiredService<UserManager<User>>(),
                    sp.GetRequiredService<IOptions<SeedOptions>>());

                await adminSeeder.SeedAsync();
            }
        }


        app.UseExceptionHandling();
        app.UseStaticFiles();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        
        app.UseRouting();

        app.UseAuthentication();

        app.UseAuthorization();

        app.MapControllers();
        return app;
    }
}