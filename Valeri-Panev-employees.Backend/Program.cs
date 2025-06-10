using Microsoft.OpenApi.Models;
using Valeri_Panev_employees.Backend.Middleware;
using Valeri_Panev_employees.Backend.Services.CsvParser;
using Valeri_Panev_employees.Backend.Services.PairService;

namespace Valeri_Panev_employees.Backend
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			ConfigureServices(builder);

			var app = builder.Build();
			Configure(app);
		}
		private static void ConfigureServices(WebApplicationBuilder builder)
		{
			builder.Services.AddHttpContextAccessor();

			builder.Services.AddCors(options =>
			{
				options.AddPolicy(
					"AllowAllOrigins",
					policy =>
						policy.AllowAnyOrigin()
							.AllowAnyMethod()
							.AllowAnyHeader());
			});

			builder.Services.AddScoped<IPairService, PairService>();
			builder.Services.AddScoped<ICsvParser, CsvParser>();
			builder.Services.AddTransient<ExceptionMiddleware>();

			builder.Services.AddControllers();
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new OpenApiInfo { Title = "Employees", Version = "v1" });
			});
		}

		private static void Configure(WebApplication app)
		{
			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}

			app.UseMiddleware<ExceptionMiddleware>();
			app.UseHttpsRedirection();
			app.UseCors("AllowAllOrigins");
			app.UseAuthentication();
			app.UseAuthorization();
			app.MapControllers();

			app.Run();
		}
	}
}
