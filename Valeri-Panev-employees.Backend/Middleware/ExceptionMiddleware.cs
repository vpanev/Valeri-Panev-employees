using System.Net;
using System.Text.Json;
using Valeri_Panev_employees.Backend.Controllers;

namespace Valeri_Panev_employees.Backend.Middleware
{
	/// <summary>
	/// Middleware component that handles exceptions globally across the application.
	/// </summary>
	public class ExceptionMiddleware : IMiddleware
	{
		private readonly ILogger<ExceptionMiddleware> _logger;
		private readonly IWebHostEnvironment _environment;

		public ExceptionMiddleware(ILogger<ExceptionMiddleware> logger, IWebHostEnvironment environment)
		{
			_logger = logger;
			_environment = environment;
		}

		/// <summary>
		/// Invokes the middleware to process an HTTP request.
		/// </summary>
		/// <param name="context">The HTTP context for the current request.</param>
		/// <param name="next">The delegate representing the next middleware in the pipeline.</param>
		/// <returns>A task that represents the completion of request processing.</returns>
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

		/// <summary>
		/// Processes an exception that occurred during request handling.
		/// </summary>
		/// <param name="context">The HTTP context for the current request.</param>
		/// <param name="exception">The exception that was thrown.</param>
		/// <returns>A task that represents the completion of exception handling.</returns>
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

		/// <summary>
		/// Maps exception types to appropriate HTTP status codes.
		/// </summary>
		/// <param name="exception">The exception to map to an HTTP status code.</param>
		/// <returns>The HTTP status code that corresponds to the exception type.</returns>
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