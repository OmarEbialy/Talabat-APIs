using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.Errors;
using Talabat.APIs.Helpers;
using Talabat.Core;
using Talabat.Core.Repositories;
using Talabat.Core.Services;
using Talabat.Repository;
using Talabat.Services;

namespace Talabat.APIs.Extensions
{
    public static class ApplicationServicesExtension
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Allow DependancyInjection for Cache Services
            services.AddSingleton<IResponseCacheService, ResponseCacheService>();
            // Singleton => hyfdl 3at4 fe memory as long as en el user lsa fat7 el app [Per User]

            // Allow DependancyInjection for Payment Service
            services.AddScoped<IPaymentServices, PaymentService>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Allow DependancyInjection for JWT Token
            services.AddScoped<ITokenService, TokenService>();


            //// Allow DependancyInjection for ProductController ONLY => Bad !
            //builder.Services.AddScoped<IGenericRepository<Product> , GenericRepository<Product>>();
            // hna b2olo y3ml Create l ay haga gaya mn IGeneric => Allow DependancyInjection = GOOD :D
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            // Allow DependancyInjection For AutoMapper
            //builder.Services.AddAutoMapper(M => M.AddProfile(new MappingProfiles())); Not Recommended
            services.AddAutoMapper(typeof(MappingProfiles)); // Recommended

            #region Validation Error Handling
            // Validation Error
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = (actionContext) =>
                {
                    var errors = actionContext.ModelState.Where(P => P.Value.Errors.Count() > 0) // P standFor Parameter
                                                         .SelectMany(P => P.Value.Errors)
                                                         .Select(E => E.ErrorMessage)            // E standFor Error
                                                         .ToArray();

                    var validationErrorResponse = new ApiValidationErrorResponse()
                    {
                        Errors = errors
                    };

                    return new BadRequestObjectResult(validationErrorResponse);
                };
            });

            #endregion

            return services;

        }
    }
}
