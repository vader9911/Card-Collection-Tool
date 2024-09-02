using Card_Collection_Tool.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

public class ScryfallSyncHostedService : IHostedService, IDisposable
{
    private readonly IServiceProvider _serviceProvider; // Inject IServiceProvider
    private Timer _timer;

    public ScryfallSyncHostedService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        // Set the timer to check every 24 hours
        _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromHours(24));
        return Task.CompletedTask;
    }

    private async void DoWork(object state)
    {
        using (var scope = _serviceProvider.CreateScope()) // Create a new scope
        {
            var syncService = scope.ServiceProvider.GetRequiredService<ScryfallSyncService>();
            await syncService.SyncScryfallDataAsync(); // Perform the sync
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}