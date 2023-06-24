namespace Talabat.APIs.Extensions
{
    public static class SwaggerServicesExtensions
    {
        public static IServiceCollection AddSwaggerServices(this IServiceCollection services)
        {
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            return services;
        }

        public static WebApplication UseSwaggerMiddlewares(this WebApplication app)
        {
            // h7ot hna el middlewares lly mwgoda f class program f swagger
            app.UseSwagger();
            app.UseSwaggerUI();

            return app;
        }
    }
}
