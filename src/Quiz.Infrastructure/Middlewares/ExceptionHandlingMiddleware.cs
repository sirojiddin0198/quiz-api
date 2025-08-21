using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Quiz.Infrastructure.Exceptions;

namespace Quiz.Infrastructure.Middlewares;

public class ExceptionHandlingMiddleware(
    ILogger<ExceptionHandlingMiddleware> logger,
    RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception occurred");

            if (context.Response.HasStarted)
            {
                logger.LogWarning("The response has already started, the exception middleware will not modify the response.");
                throw;
            }

            (int code, string message) = ex switch
            {
                CustomBadRequestException badRequest => ((int)HttpStatusCode.BadRequest, badRequest.Message),
                CustomConflictException conflict => ((int)HttpStatusCode.Conflict, conflict.Message),
                CustomNotFoundException notFound => ((int)HttpStatusCode.NotFound, notFound.Message),
                CustomUnauthorizedException unauthorized => ((int)HttpStatusCode.Unauthorized, unauthorized.Message),
                _ => ((int)HttpStatusCode.InternalServerError, "An unexpected error occurred.")
            };

            context.Response.Clear();
            context.Response.StatusCode = code;
            context.Response.ContentType = "application/json";

            var errorObj = new
            {
                statusCode = code,
                error = message
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(errorObj));
        }
    }
}