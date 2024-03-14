namespace Dinex.AppService;

public static class DependencyInjection
{
    public static IServiceCollection AddBusinessServices(this IServiceCollection services)
    {
        services.AddInfraServices();

        services.AddScoped<IUserAppService, UserAppService>();
        services.AddScoped<IFileAppService, FileAppService>();
        services.AddScoped<IQueueAppService, QueueAppService>();
        services.AddScoped<IProcessingService, ProcessingService>();

        return services;
    }
}
