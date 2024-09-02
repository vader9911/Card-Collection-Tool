using Card_Collection_Tool.Services;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

public class ScryfallSyncHostedService : IHostedService, IDisposable
{
    private readonly ScryfallSyncService _syncService;
    private Timer _timer;

    public ScryfallSyncHostedService(ScryfallSyncService syncService)
    {
        _syncService = syncService;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromHours(1)); // Check data freshness every hour
        return Task.CompletedTask;
    }

    private async void DoWork(object state)
    {
        await _syncService.SyncScryfallDataAsync(); // Attempt to sync
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