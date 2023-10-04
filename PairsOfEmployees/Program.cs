using PairsOfEmployees.Common;
using PairsOfEmployees.Configuration;
using PairsOfEmployees.Services;

namespace PairsOfEmployees
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var serviceConfig = builder.Configuration.GetSection("ServiceConfig");
            builder.Services.Configure<ServiceConfiguration>(serviceConfig);
            
            builder.Services.AddSingleton<ICsvHelper, CsvHelper>();
            builder.Services.AddSingleton<IEmployeeExtractorService, EmployeeExtractorService>();

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", policy =>
                {
                    var origins = builder.Configuration
                        .GetSection("AllowedOrigins")
                        .Get<string[]>();

                    policy
                        .WithOrigins(origins!)
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.UseCors("CorsPolicy");

            app.MapControllers();

            app.Run();
        }
    }
}