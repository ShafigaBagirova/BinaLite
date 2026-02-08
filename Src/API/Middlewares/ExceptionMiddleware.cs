using Application.Shared.Helpers.Responses;
using System.Net;
using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Http.Json;

namespace API.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
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
            context.Response.ContentType = "application/ison";
            var message = ex.Errors.FirstOrDefault()?.ErrorMessage ?? "Validation failed";
            var response = BaseResponse.Fail(message);
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
        catch (Exception)
        {

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";
            var response = BaseResponse.Fail("Server error");
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));

        }
    }
}
