using CLINICAL_MANAGEMENT.Models;
using CLINICAL_MANAGEMENT.Repository;
using CLINICAL_MANAGEMENT.Service;
using Microsoft.EntityFrameworkCore;

namespace CLINICAL_MANAGEMENT
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add DbContext
            builder.Services.AddDbContext<CmsContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("ApiConnection")));

            // DI
            builder.Services.AddScoped<IReceptionRepository, ReceptionRepositoryImpl>();
            builder.Services.AddScoped<IReceptionService, ReceptionServiceImpl>();

            // Controllers
            builder.Services.AddControllers()
                    .AddJsonOptions(options =>
                    {
                        options.JsonSerializerOptions.ReferenceHandler =
                            System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
                    });

            // Swagger
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // ? ADD CORS HERE
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAngular",
                    policy =>
                    {
                        policy.WithOrigins("http://localhost:4200")
                              .AllowAnyHeader()
                              .AllowAnyMethod();
                    });
            });

            var app = builder.Build();

            // Swagger
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            // ? USE CORS HERE (VERY IMPORTANT POSITION)
            app.UseCors("AllowAngular");

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}