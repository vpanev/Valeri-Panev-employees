using System.Net;
using System.Text.Json;
using Valeri_Panev_employees.Backend.Controllers;

namespace Valeri_Panev_employees.Backend.Middleware
{
	public class ExceptionMiddleware : IMiddleware
	{
		private readonly ILogger<ExceptionMiddleware> _logger;
		private readonly IWebHostEnvironment _environment;

		public ExceptionMiddleware(ILogger<ExceptionMiddleware> logger, IWebHostEnvironment environment)
		{
			_logger = logger;
			_environment = environment;
		}

		public async Task InvokeAsync(HttpContext context, RequestDelegate next)
		{
			try
			{
				await next(context);
			}
			catch (Exception ex)
			{
				await HandleExceptionAsync(context, ex);
			}
		}

		private async Task HandleExceptionAsync(HttpContext context, Exception exception)
		{
			_logger.LogError(exception, "An unhandled exception occurred");

			var statusCode = GetStatusCode(exception);

			context.Response.ContentType = "application/json";
			context.Response.StatusCode = (int) statusCode;

			var errorMessage = exception.Message;
			if (statusCode == HttpStatusCode.InternalServerError && !_environment.IsDevelopment())
			{
				errorMessage = "An unexpected error occurred";
			}

			var response = ApiWrapper<object>.CreateError(errorMessage);

			if (_environment.IsDevelopment())
			{
				response.Errors.Add($"Stack Trace: {exception.StackTrace}");
			}

			var jsonOptions = new JsonSerializerOptions
			{
				PropertyNamingPolicy = JsonNamingPolicy.CamelCase
			};

			var json = JsonSerializer.Serialize(response, jsonOptions);
			await context.Response.WriteAsync(json);
		}

		private static HttpStatusCode GetStatusCode(Exception exception)
		{
			return exception switch
			{
				ArgumentException _ => HttpStatusCode.BadRequest,
				UnauthorizedAccessException _ => HttpStatusCode.Unauthorized,
				InvalidOperationException _ => HttpStatusCode.BadRequest,
				// Add more exception mappings as needed
				_ => HttpStatusCode.InternalServerError
			};
		}
	}
}