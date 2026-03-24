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
            builder.Services.AddScoped<IReceptionRepository, ReceptionRepositoryImpl>();
            builder.Services.AddScoped<IReceptionService, ReceptionServiceImpl>();
            // Add services to the container.

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
