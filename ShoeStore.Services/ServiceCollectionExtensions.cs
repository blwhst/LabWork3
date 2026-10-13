using Microsoft.Extensions.DependencyInjection;
using ShoeStore.Services.Implementations;
using ShoeStore.Services.Interfaces;

namespace ShoeStore.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IReferenceService, ReferenceService>();
        return services;
    }
}