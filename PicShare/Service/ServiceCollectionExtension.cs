using Common.Dto;
using Microsoft.Extensions.DependencyInjection;
using Repository;
using Service.Interfaces;
using Service.Services;

namespace Service
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddRepository();
            services.AddScoped<IPhotoService, PhotoService>();
            services.AddScoped<IUserService, UserService>();
            services.AddAutoMapper(typeof(MapProfile));
            return services;
        }
    }
}
