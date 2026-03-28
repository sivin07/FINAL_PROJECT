using CLINICAL_MANAGEMENT.Repositories;
using CLINICAL_MANAGEMENT.Services;
using CLINICAL_MANAGEMENT.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace CLINICAL_MANAGEMENT
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // JSON Configuration
            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = null;
                    options.JsonSerializerOptions.WriteIndented = true;
                    options.JsonSerializerOptions.Converters.Add(
                        new JsonStringEnumConverter());
                    options.JsonSerializerOptions.DefaultIgnoreCondition =
                        JsonIgnoreCondition.WhenWritingNull;
                    options.JsonSerializerOptions.ReferenceHandler =
                        ReferenceHandler.IgnoreCycles;
                });

            // DbContext
            builder.Services.AddDbContext<CmsContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // ───────── Lab Technician Module ─────────
            builder.Services.AddScoped<ILabTechnicianRepository, LabTechRepositoryImpl>();
            builder.Services.AddScoped<ILabTechnicianService, LabTechServiceImpl>();

            // ───────── Doctor Module ─────────
            builder.Services.AddScoped<IDoctorRepository, DoctorRepoImpl>();
            builder.Services.AddScoped<IDoctorService, DoctorServiceImpl>();

            // ───────── Pharmacist Module ─────────
            builder.Services.AddScoped<IPharmacistRepository, PharmacistRepoImpl>();
            builder.Services.AddScoped<IPharmacistService, PharmacistServiceImpl>();

            // Swagger
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Middleware
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