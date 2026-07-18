using System.Text.Json.Serialization;
using EducationCenter.Api.Data;
using Microsoft.EntityFrameworkCore;
namespace EducationCenter.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.


            builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
            {
                options.SerializerOptions.NumberHandling =
                    JsonNumberHandling.Strict;
            });

            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.NumberHandling =
                        JsonNumberHandling.Strict;
                });

            builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(
                builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddOpenApi();

            var app = builder.Build();
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();

                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint(
                        "/openapi/v1.json",
                        "Education Center API");
                });
            }
            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
