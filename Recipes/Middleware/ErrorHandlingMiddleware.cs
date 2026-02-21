using System.Net;
using System.Text.Json;

namespace Recipes.Middleware;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception occurred");

            var response = context.Response;
            response.ContentType = "application/json";

            var statusCode = MapExceptionToStatusCode(ex);
            response.StatusCode = (int)statusCode;

            var result = JsonSerializer.Serialize(new
            {
                status = response.StatusCode,
                error = GetMessageForStatusCode(statusCode)
            });

            await response.WriteAsync(result);
        }
    }

    private static HttpStatusCode MapExceptionToStatusCode(Exception ex)
    {
        HttpStatusCode code;
        
        switch (ex)
        {
            case UnauthorizedAccessException:
                code = HttpStatusCode.Unauthorized;
                break;
            case KeyNotFoundException:
                code = HttpStatusCode.NotFound;
                break;
            case InvalidOperationException:
                code = HttpStatusCode.Conflict;
                break;
            case ArgumentException:
                code = HttpStatusCode.BadRequest;
                break;
            case NotSupportedException:
                code = HttpStatusCode.BadRequest;
                break;
            default:
                code = HttpStatusCode.InternalServerError;
                break;
        }

        return code;
    }

    private static string GetMessageForStatusCode(HttpStatusCode code)
    {
        string message;
        
        switch (code)
        {
            case HttpStatusCode.Forbidden:
                message = "Access Denied.";
                break;
            case HttpStatusCode.NotFound:
                message = "not found.";
                break;
            case HttpStatusCode.Conflict:
                message = "The item already exists.";
                break;
            case HttpStatusCode.BadRequest:
                message = "One or more system errors have occured.";
                break;
            case HttpStatusCode.InternalServerError:
                message = "An unexpected error occurred.";
                break;
            default:
                message = "An unexpected error occurred.";
                break;
        }
        ;

        return message;
    }
}