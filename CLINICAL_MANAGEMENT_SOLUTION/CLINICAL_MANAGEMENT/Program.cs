using CLINICAL_MANAGEMENT.Models;
using CLINICAL_MANAGEMENT.Repositories;
using CLINICAL_MANAGEMENT.Repository;
using CLINICAL_MANAGEMENT.Service;
using CLINICAL_MANAGEMENT.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
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
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                    options.JsonSerializerOptions.DefaultIgnoreCondition =
                        JsonIgnoreCondition.WhenWritingNull;
                    options.JsonSerializerOptions.ReferenceHandler =
                        ReferenceHandler.IgnoreCycles;
                });

            // CORS
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

            // DbContext
            builder.Services.AddDbContext<CmsContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // 🔐 AUTH MODULE (from testing branch)
            builder.Services.AddScoped<IAuthRepository, AuthRepoImpl>();
            builder.Services.AddScoped<IAuthService, AuthServiceImpl>();

            // 🧪 LAB TECH
            builder.Services.AddScoped<ILabTechnicianRepository, LabTechRepositoryImpl>();
            builder.Services.AddScoped<ILabTechnicianService, LabTechServiceImpl>();

            // 👨‍⚕️ DOCTOR
            builder.Services.AddScoped<IDoctorRepository, DoctorRepoImpl>();
            builder.Services.AddScoped<IDoctorService, DoctorServiceImpl>();

            // 💊 PHARMACIST
            builder.Services.AddScoped<IPharmacistRepository, PharmacistRepoImpl>();
            builder.Services.AddScoped<IPharmacistService, PharmacistServiceImpl>();

            // 🧾 RECEPTION (YOUR MODULE)
            builder.Services.AddScoped<IReceptionRepository, ReceptionRepositoryImpl>();
            builder.Services.AddScoped<IReceptionService, ReceptionServiceImpl>();

            // 🔐 JWT CONFIG
            var jwtSettings = builder.Configuration.GetSection("Jwt");

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                };
            });

            // Swagger
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            // CORS
            app.UseCors("AllowAngular");

            // 🔐 Authentication (IMPORTANT - was missing)
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}