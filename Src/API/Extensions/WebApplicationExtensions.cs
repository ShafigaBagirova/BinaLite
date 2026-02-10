using API.Middlewares;

namespace API.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        app.UseExceptionHandling();
        app.UseStaticFiles();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        // ✅ MÜTLƏQ əvvəl Authentication
        app.UseAuthentication();

        // ✅ sonra Authorization (1 dəfə!)
        app.UseAuthorization();

        app.MapControllers();
        return app;
    }
}