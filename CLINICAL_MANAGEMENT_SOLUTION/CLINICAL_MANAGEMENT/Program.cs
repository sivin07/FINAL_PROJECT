using CLINICAL_MANAGEMENT.Models;
using CLINICAL_MANAGEMENT.Repositories;
using CLINICAL_MANAGEMENT.Repository;
using CLINICAL_MANAGEMENT.Service;
using CLINICAL_MANAGEMENT.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
<<<<<<< HEAD
=======
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;
>>>>>>> origin/testing

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

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAngular",
                    policy =>
                    {
                        policy.WithOrigins("http://localhost:4200") // Angular URL
                              .AllowAnyHeader()
                              .AllowAnyMethod();
                    });
            });

            // DbContext (ONLY ONE)
            builder.Services.AddDbContext<CmsContext>(options =>
<<<<<<< HEAD
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
=======
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddScoped<IAuthRepository, AuthRepoImpl>();
            builder.Services.AddScoped<IAuthService, AuthServiceImpl>();

            // ───────── Lab Technician Module ─────────
            builder.Services.AddScoped<ILabTechnicianRepository, LabTechRepositoryImpl>();
            builder.Services.AddScoped<ILabTechnicianService, LabTechServiceImpl>();

            // ───────── Doctor Module ─────────
            builder.Services.AddScoped<IDoctorRepository, DoctorRepoImpl>();
            builder.Services.AddScoped<IDoctorService, DoctorServiceImpl>();

            // ───────── Pharmacist Module ─────────
            builder.Services.AddScoped<IPharmacistRepository, PharmacistRepoImpl>();
            builder.Services.AddScoped<IPharmacistService, PharmacistServiceImpl>();

            // ───────── Reception Module ─────────
            builder.Services.AddScoped<IReceptionRepository, ReceptionRepositoryImpl>();
            builder.Services.AddScoped<IReceptionService, ReceptionServiceImpl>();



       
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

                    ValidIssuer = jwtSettings["Jwt:Issuer"],
                    ValidAudience = jwtSettings["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                };
            });
>>>>>>> origin/testing

            // Swagger
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

<<<<<<< HEAD
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
=======
            var app = builder.Build();

            // Middleware
>>>>>>> origin/testing
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
<<<<<<< HEAD

            // ? USE CORS HERE (VERY IMPORTANT POSITION)
            app.UseCors("AllowAngular");

            app.UseAuthorization();

=======
            app.UseCors("AllowAngular");
            app.UseCors("AllowAngular");
            app.UseAuthorization();
>>>>>>> origin/testing
            app.MapControllers();
            app.Run();
        }
    }
}