using System.Text;
using TikkoPayCallbacks.Infrastructure.Helpers;
using TikkoPayCallbacks.Infrastructure.Repositories.Interfaces;
using TikkoPayCallbacks.Infrastructure.Services.Interfaces;

namespace TikkoPayCallbacks.Jobs
{
    public class WebSiteBakiyeUnder100kTask : BackgroundService
    {
        private readonly ILogger<WebSiteBakiyeUnder100kTask> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly TimeSpan _interval = TimeSpan.FromMinutes(10);
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public WebSiteBakiyeUnder100kTask(ILogger<WebSiteBakiyeUnder100kTask> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("WebSiteBakiyeUnder100kTask started at: {time}", DateTimeOffset.Now);

            using var timer = new PeriodicTimer(_interval);

            try
            {
                do
                {
                    if (stoppingToken.IsCancellationRequested)
                        break;

                    if (!_semaphore.Wait(0))
                    {
                        _logger.LogWarning("Önceki görev hâlâ çalışıyor, bu döngü atlandı. Zaman: {time}", DateTimeOffset.Now);
                        continue;
                    }

                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            _logger.LogInformation("Görev başladı. Zaman: {time}", DateTimeOffset.Now);
                            await DoWorkAsync(stoppingToken);
                            _logger.LogInformation("Görev tamamlandı. Zaman: {time}", DateTimeOffset.Now);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Görev yürütülürken hata oluştu.");
                        }
                        finally
                        {
                            _semaphore.Release();
                        }
                    }, stoppingToken);
                }
                while (await timer.WaitForNextTickAsync(stoppingToken));
            }
            catch (OperationCanceledException)
            {
                // uygulama kapanıyor
            }
            finally
            {
                _logger.LogInformation("WebSiteBakiyeUnder100kTask stopped at: {time}", DateTimeOffset.Now);
            }
        }

        private async Task DoWorkAsync(CancellationToken cancellationToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var webSiteRepository = scope.ServiceProvider.GetRequiredService<IWebSiteRepository>();
            var telegramBotService = scope.ServiceProvider.GetRequiredService<ITelegramBotService>();

            StringBuilder stringBuilder = new StringBuilder();

            var webSiteleri = await webSiteRepository.GetAllAsync();
            foreach (var website in webSiteleri)
            {
                var websiteBakiye = await webSiteRepository.GetWebsiteBakiyeAsync(website.Id);
                var remainingAmount = website.BakiyeLimitor - websiteBakiye;
                if (remainingAmount >= -100000)
                {
                    stringBuilder.AppendLine($"Website: {website.Baslik} limitöre 100k. BakiyeLimitör: {website.BakiyeLimitor.ToString("N2")}, Bakiye: {websiteBakiye.ToString("N2")}");
                }
            }

            var telegramMessage = stringBuilder.ToString();
            await telegramBotService.SendLongMessageAsync(TelegramBotConstants.AnlikBakiyelerChannelId, telegramMessage, 4000, cancellationToken);
        }
    }
}
