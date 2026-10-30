using System.Text;
using TikkoPayCallbacks.Infrastructure.Helpers;
using TikkoPayCallbacks.Infrastructure.Repositories.Interfaces;
using TikkoPayCallbacks.Infrastructure.Services.Interfaces;

namespace TikkoPayCallbacks.Jobs
{
    public class TedarikciWebSiteBakiyeTask : BackgroundService
    {
        private readonly ILogger<TedarikciWebSiteBakiyeTask> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly TimeSpan _interval = TimeSpan.FromMinutes(20);
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public TedarikciWebSiteBakiyeTask(ILogger<TedarikciWebSiteBakiyeTask> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("TedarikciWebSiteBakiyeTask started at: {time}", DateTimeOffset.Now);

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
                _logger.LogInformation("TedarikciWebSiteBakiyeTask stopped at: {time}", DateTimeOffset.Now);
            }
        }

        private async Task DoWorkAsync(CancellationToken cancellationToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var tedarikciRepository = scope.ServiceProvider.GetRequiredService<ITedarikcilerRepository>();
            var websiteRepository = scope.ServiceProvider.GetRequiredService<IWebSiteRepository>();
            var telegramBotService = scope.ServiceProvider.GetRequiredService<ITelegramBotService>();

            await SendTedarikciBakiye(tedarikciRepository, telegramBotService, cancellationToken);
            
            await Task.Delay(200, cancellationToken);

            await SendWebsiteBakiye(websiteRepository, telegramBotService, cancellationToken);

            _logger.LogInformation("DoWorkAsync çalıştı. Zaman: {time}", DateTimeOffset.Now);
        }

        private async Task SendTedarikciBakiye(ITedarikcilerRepository tedarikciRepository, ITelegramBotService telegramBotService, CancellationToken cancellationToken)
        {
            StringBuilder stringBuilder = new StringBuilder();

            var tedarikciList = await tedarikciRepository.GetTedarikciler();
            foreach (var tedarikci in tedarikciList)
            {
                var tedarikciBakiye = await tedarikciRepository.GetTedarikciBakiye(tedarikci.Id);

                stringBuilder.AppendLine($"Tedarikci: {tedarikci.Unvan} - Bakiye: {(tedarikciBakiye.ToString("N2"))} ({(tedarikciBakiye < 0 ? "Alacaklı" : "Borçlu")})");
            }

            var telegramMessage = stringBuilder.ToString();
            await telegramBotService.SendLongMessageAsync(TelegramBotConstants.AnlikBakiyelerChannelId, telegramMessage, 4000, cancellationToken);
        }

        private async Task SendWebsiteBakiye(IWebSiteRepository webSiteRepository, ITelegramBotService telegramBotService, CancellationToken cancellationToken)
        {
            StringBuilder stringBuilder = new StringBuilder();

            var webSiteleri = await webSiteRepository.GetAllAsync();
            foreach (var website in webSiteleri)
            {
                var websiteBakiye = await webSiteRepository.GetWebsiteBakiyeAsync(website.Id);

                stringBuilder.AppendLine($"Website: {website.Baslik} - Bakiye: {(websiteBakiye.ToString("N2"))} ({(websiteBakiye < 0 ? "Borçlu" : "Alacaklı")})");
            }

            var telegramMessage = stringBuilder.ToString();
            await telegramBotService.SendLongMessageAsync(TelegramBotConstants.AnlikBakiyelerChannelId, telegramMessage, 4000, cancellationToken);
        }
    }
}
