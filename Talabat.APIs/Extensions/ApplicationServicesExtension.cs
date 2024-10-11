using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.Errors;
using Talabat.APIs.Helpers;
using Talabat.Core.Repositories;
using Talabat.Core.Repositories.contract;
using Talabat.Core.Services.contract;
using Talabat.Repository;
using Talabat.Repository.Data;
using Talabat.Service;

namespace Talabat.APIs.Extensions
{
    public static class ApplicationServicesExtension
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));

            services.AddScoped<IOrderService, OrderService>();

            services.AddAutoMapper(typeof(MappingProfiles));

            #region Validation Error
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = (actioncontext) =>
                {
                    var errors = actioncontext.ModelState.Where(p => p.Value.Errors.Count() > 0)
                                                                      .SelectMany(p => p.Value.Errors)
                                                                      .Select(e => e.ErrorMessage)
                                                                      .ToList();

                    var response = new ApiValidationErrorResponse
                    {
                        Errors = errors
                    };

                    return new BadRequestObjectResult(response);
                };
            });
            #endregion

            services.AddScoped(typeof(IBasketRepository), typeof(BasketRepository));

            return services;
        }

        public static IServiceCollection AddSwaggerServices(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            //services.AddSwaggerGen(options =>
            //{

            //    options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
            //});

            return services;
        }

        public static WebApplication UseSwaggerMiddleware(this WebApplication app)
        {
            app.UseSwagger();
            app.UseSwaggerUI();

            return app;
        }
    }
}
