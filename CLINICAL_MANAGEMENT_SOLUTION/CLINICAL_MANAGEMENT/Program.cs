
using CLINICAL_MANAGEMENT.Models;
using CLINICAL_MANAGEMENT.Repositories;
using CLINICAL_MANAGEMENT.Services;
using Microsoft.EntityFrameworkCore;

namespace CLINICAL_MANAGEMENT
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // JSON Format
            builder.Services.AddControllersWithViews()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = null;
                    options.JsonSerializerOptions.WriteIndented = true;  // Readability
                });

            // Add services to the container.

            // 1- DbContext and connection string registration as middleware
            builder.Services.AddDbContext<CmsContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("WebApiDBConnection")));

            // 2- Service and Repository registration as middleware
            builder.Services.AddScoped<ILabTechnicianRepository, LabTechRepositoryImpl>();
            builder.Services.AddScoped<ILabTechnicianService, LabTechServiceImpl>();

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
