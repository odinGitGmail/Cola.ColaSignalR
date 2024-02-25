using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cola.ColaSignalR;

public static class ColaSignalRHubInject
{
    /// <summary>
    /// AddColaNacos
    /// </summary>
    /// <param name="services">services</param>
    /// <returns></returns>
    public static IServiceCollection AddColaSignalRHub(
        this IServiceCollection services)
    {
        services.AddSignalR().AddNewtonsoftJsonProtocol();
        return services;
    }
}