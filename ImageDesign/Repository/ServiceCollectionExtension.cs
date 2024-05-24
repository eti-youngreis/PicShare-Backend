using Microsoft.Extensions.DependencyInjection;
using Repository.Entity;
using Repository.Interfaces;
using Repository.Repository;

namespace Repository
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddRepository(this IServiceCollection services)
        {
            services.AddScoped<IRepository<DesignedImage>, DesignedImageRepository>();
            services.AddScoped<IRepository<DesignTemplate>, DesignTemplateRepository>();
            services.AddScoped<IRepository<Image>, ImageRepository>();
            services.AddScoped<IRepository<User>, UserRepository>();
            return services;
        }
    }
}
