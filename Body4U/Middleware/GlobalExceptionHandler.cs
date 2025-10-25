namespace Body4U.Membership.Api.Middleware
{
    using System.Net;
    using System.Text.Json;
    using FluentValidation;

    public class GlobalExceptionHandler
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(
            RequestDelegate next,
            ILogger<GlobalExceptionHandler> logger)
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            int statusCode;
            object response;

            if (exception is ValidationException validationEx)
            {
                statusCode = (int)HttpStatusCode.BadRequest;
                response = new
                {
                    statusCode,
                    message = "Validation failed",
                    errors = validationEx.Errors.Select(e => new { e.PropertyName, e.ErrorMessage })
                };
            }
            else if (exception is InvalidOperationException)
            {
                statusCode = (int)HttpStatusCode.BadRequest;
                response = new
                {
                    statusCode,
                    message = exception.Message
                };
            }
            else
            {
                statusCode = (int)HttpStatusCode.InternalServerError;
                response = new
                {
                    statusCode,
                    message = "An internal server error occurred"
                };
            }

            context.Response.StatusCode = statusCode;
            return context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}