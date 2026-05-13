using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ElBruno.CopilotCLIMonitor.Core.Interfaces;
using ElBruno.CopilotCLIMonitor.Core.Services;

namespace ElBruno.CopilotCLIMonitor.Core.Infrastructure;

/// <summary>
/// Registers Core library services into an <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds all Core services using the supplied <paramref name="settings"/> (or defaults).
    /// </summary>
    /// <remarks>
    /// Registers:
    /// <list type="bullet">
    ///   <item><see cref="IEventStore"/> → <see cref="EventStore"/> (singleton)</item>
    ///   <item><see cref="IRepositoryDetector"/> → <see cref="RepositoryDetector"/> (singleton)</item>
    ///   <item><see cref="IHookInstaller"/> → <see cref="HookInstaller"/> (singleton)</item>
    ///   <item><see cref="IIpcClient"/> → <see cref="HttpIpcClient"/> (singleton, port from settings)</item>
    ///   <item><see cref="IMonitorEventParser"/> → <see cref="MonitorEventParser"/> (singleton)</item>
    ///   <item><see cref="CoreSettings"/> (singleton)</item>
    /// </list>
    /// </remarks>
    public static IServiceCollection AddCopilotCLIMonitorCore(
        this IServiceCollection services,
        CoreSettings? settings = null)
    {
        var resolved = settings ?? CoreSettings.Default;

        services.TryAddSingleton(resolved);
        services.TryAddSingleton<IEventStore>(sp =>
            new EventStore(
                resolved.EventStoreCapacity,
                sp.GetService<Microsoft.Extensions.Logging.ILogger<EventStore>>()));
        services.TryAddSingleton<IRepositoryDetector, RepositoryDetector>();
        services.TryAddSingleton<IHookInstaller, HookInstaller>();
        services.TryAddSingleton<IIpcClient>(_ => new HttpIpcClient(resolved.IpcPort, TimeSpan.FromSeconds(resolved.IpcTimeoutSeconds)));
        services.TryAddSingleton<IMonitorEventParser, MonitorEventParser>();

        return services;
    }
}
