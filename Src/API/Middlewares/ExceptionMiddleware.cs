using Application.Shared.Helpers.Responses;
using System.Net;
using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Http.Json;

namespace API.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.ContentType = "application/json";

            var errors = ex.Errors
                .Select(e => e.ErrorMessage)
                .Distinct()
                .ToList();

            var message = errors.FirstOrDefault() ?? "Validation failed";

            var response = BaseResponse.Fail(message, errors);

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception. Path: {Path}", context.Request.Path);

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";

            var response = BaseResponse.Fail("Server error");
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
