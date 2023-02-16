using EvolveDb;
using Microsoft.Data.SqlClient;

namespace TestApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var configuration = builder.Configuration;

            var app = builder.Build();

            MigrateDatabase(configuration.GetConnectionString("test"), app.Logger);

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }

        private static void MigrateDatabase(string connectionString, ILogger logger)
        {
            try
            {
                var cnx = new SqlConnection(connectionString);
                var evolve = new Evolve(cnx, msg => logger.LogInformation(msg))
                {
                    Locations = new[] { "Migrations" },
                    IsEraseDisabled = true,
                };

                //need to fix the issue related to checksum
                //evolve.Repair();
                evolve.Migrate();
            }
            catch (Exception ex)
            {
                logger.LogError("Database migration failed.", ex.Message);
                throw;
            }
        }
    }
}