namespace ConcordiaMVC;

using ConcordiaUtilsLibrary;
using ConcordiaTrelloLibrary;

public class SynchronizationBackgroundService : BackgroundService
{
    private int _syncIntervalMinutes;
    private string _urlBoard;

    public bool IsSynchronizing { get; private set; }

    public SynchronizationBackgroundService(IConfiguration configuration)
    {
        _syncIntervalMinutes = configuration.GetValue<int>("SynchronizationBackgroundService:SyncIntervalMinutes");
        _urlBoard = configuration.GetValue<string>("LinkTrelloBoard:Url");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Startup startup = new Startup();
        while (!stoppingToken.IsCancellationRequested)
        {
            if (await TrelloSettings.IsBoardAccessibleAsync())
            {
                IsSynchronizing = true;
                await startup.RunAsync();
                IsSynchronizing = false;
                await Task.Delay(TimeSpan.FromMinutes(_syncIntervalMinutes), stoppingToken);
            }
        }
    }
}