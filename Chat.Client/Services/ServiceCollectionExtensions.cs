using Chat.Client.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace Chat.Client.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddChatServices(this IServiceCollection services)
    {
        return services.AddSingleton<IpConfigService>();
    }

    public static IServiceCollection AddGuiServices(this IServiceCollection services)
    {
        return services
            .AddSingleton<MainWindowViewModel>()
            .AddTransient<ChatViewModel>();
    }
}