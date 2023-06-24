using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using Talabat.APIs.Errors;
using Talabat.APIs.Extensions;
using Talabat.APIs.Helpers;
using Talabat.APIs.Middlewares;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Identity;
using Talabat.Core.Repositories;
using Talabat.Repository;
using Talabat.Repository.Data;
using Talabat.Repository.Identity;

namespace Talabat.APIs
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            #region Configure Services

            builder.Services.AddControllers(); // Allow API Services

            builder.Services.AddSwaggerServices();

            // Make APIs Project Talk to Repo Layer to Allow DependancyInjection for DbContext
            // Scoped => Per Request
            builder.Services.AddDbContext<StoreContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
                // Type ConnectionString in AppSettingFile in Project APIs
            });

            builder.Services.AddDbContext<AppIdentityDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection"));
                // Type ConnectionString in AppSettingFile in Project APIs
            });

            // Allow Dependacny Injection For Redis

            builder.Services.AddSingleton<IConnectionMultiplexer>(options =>
            {
                var connection = builder.Configuration.GetConnectionString("Redis");
                return ConnectionMultiplexer.Connect(connection);
            });

            // Allow Dependacny Injection For IBasketRepository

            builder.Services.AddScoped(typeof(IBasketRepository) , typeof(BasketRepository));


            // Calling Services of Application
            builder.Services.AddApplicationServices();

            builder.Services.AddIdentityServices(builder.Configuration);

            // Cors Dependancy Injection

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("MyPolicy", options =>
                {
                    options.AllowAnyHeader().AllowAnyMethod().WithOrigins(builder.Configuration["FrontBaseUrl"]);
                });
            });

            #endregion

            var app = builder.Build();

            #region Update Database
            // Using => to Make Dispose()
            using var scope = app.Services.CreateScope();

            var services = scope.ServiceProvider;

            var loggerFactory = services.GetRequiredService<ILoggerFactory>(); // make error shown in Kestrel Screen

            try
            {
                var dbContext = services.GetRequiredService<StoreContext>(); // Ask Explicity CLR to Make Object from StoreContext

                await dbContext.Database.MigrateAsync(); // Apply Migration

                // Calling Seeds
                await StoreContextSeed.SeedAsync(dbContext);

                // Update DataBase for IdentityUser
                var identityContext = services.GetRequiredService<AppIdentityDbContext>();
                await identityContext.Database.MigrateAsync();

                // Call Seeding of UserIdentity
                var userManager = services.GetRequiredService<UserManager<AppUser>>();
                await AppIdentityDbContextSeed.SeedUsersAsync(userManager);

                // Update Migration for OrderModule


            }
            catch (Exception ex)
            {
                var logger = loggerFactory.CreateLogger<Program>();
                logger.LogError(ex, " An Error Occured During Applying Migrations !");
            }

            #endregion


            #region Configure Middilewares

            app.UseMiddleware<ExceptionMiddleware>();


            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                // Calling function from SwaggerServicesExtensions
                app.UseSwaggerMiddlewares();
            }

            // Handling NotFound Error
            app.UseStatusCodePagesWithReExecute("/erros/{0}");

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseCors("MyPolicy"); // Allow Other Origins [Other Projects] Can Talk to OUR Api Project

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            #endregion

            app.Run();
        }
    }
}





