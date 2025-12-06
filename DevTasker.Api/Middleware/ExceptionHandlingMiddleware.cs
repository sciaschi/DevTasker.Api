using DevTasker.Domain.Utilities;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DevTasker.Api.Middleware
{
    public class ErrorResponse
    {
        [JsonPropertyName("code")]
        public string Code { get; set; } = "UNEXPECTED_ERROR";

        [JsonPropertyName("message")]
        public string Message { get; set; } = "An unexpected error occurred.";

        [JsonPropertyName("details")]
        public object? Details { get; set; }
    }

    public class Envelope<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public ErrorResponse? Error { get; set; }
    }

    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionHandlingMiddleware(RequestDelegate next,
            ILogger<ExceptionHandlingMiddleware> logger, IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            var statusCode = HttpStatusCode.InternalServerError;
            var error = new ErrorResponse();

            switch (ex)
            {
                case NotFoundException nf:
                    statusCode = HttpStatusCode.NotFound;
                    error.Code = "NOT_FOUND";
                    error.Message = nf.Message;
                    break;
                case ValidationException ve:
                    statusCode = HttpStatusCode.BadRequest;
                    error.Code = "VALIDATION_ERROR";
                    error.Message = ve.Message;
                    break;
                case ConflictException ce:
                    statusCode = HttpStatusCode.Conflict;
                    error.Code = "CONFLICT";
                    error.Message = ce.Message;
                    break;
                case ForbiddenException fe:
                    statusCode = HttpStatusCode.Forbidden;
                    error.Code = "FORBIDDEN";
                    error.Message = fe.Message;
                    break;
                default:
                    statusCode = HttpStatusCode.InternalServerError;
                    error.Code = "UNEXPECTED_ERROR";
                    error.Message = "An unexpected error occurred.";
                    break;
            }

            if (_env.IsDevelopment())
            {
                error.Details = new
                {
                    ex.GetType().FullName,
                    ex.StackTrace
                };
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var json = JsonSerializer.Serialize(new {
                error
            });

            await context.Response.WriteAsync(json);
        }
    }
}
