using System.Text;
using TikkoPayCallbacks.Infrastructure.Helpers;
using TikkoPayCallbacks.Infrastructure.Repositories.Interfaces;
using TikkoPayCallbacks.Infrastructure.Services.Interfaces;

namespace TikkoPayCallbacks.Jobs
{
    public class TedarikciHesapListTask : BackgroundService
    {
        private readonly ILogger<TedarikciHesapListTask> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly TimeSpan _interval = TimeSpan.FromMinutes(10);
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public TedarikciHesapListTask(ILogger<TedarikciHesapListTask> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("TedarikciHesapListTask started at: {time}", DateTimeOffset.Now);

            using var timer = new PeriodicTimer(_interval);

            try
            {
                while (await timer.WaitForNextTickAsync(stoppingToken))
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
            }
            catch (OperationCanceledException)
            {
                // uygulama kapanıyor
            }
            finally
            {
                _logger.LogInformation("TedarikciHesapListTask stopped at: {time}", DateTimeOffset.Now);
            }
        }

        private async Task DoWorkAsync(CancellationToken cancellationToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var tedarikciRepository = scope.ServiceProvider.GetRequiredService<ITedarikcilerRepository>();
            var telegramBotService = scope.ServiceProvider.GetRequiredService<ITelegramBotService>();

            StringBuilder stringBuilder = new StringBuilder();

            var tedarikciHavaleBankList = await tedarikciRepository.GetTedarikciHavaleBankalari();
            foreach (var item in tedarikciHavaleBankList)
            {
                stringBuilder.AppendLine($"Tedarikci: {item.Tedarikci}");
                stringBuilder.AppendLine($"Tedarikci Bakiye Limitör: {item.TedarikciBakiyeLimitor}");
                stringBuilder.AppendLine($"Banka: {item.BankaAdi}");
                stringBuilder.AppendLine($"Hesap Min Tutar: {item.HesapMinTutar.ToString("N2")}");
                stringBuilder.AppendLine($"Hesap Max Tutar: {item.HesapMaxTutar.ToString("N2")}");
                stringBuilder.AppendLine($"Hesap Sahibi: {item.HesapSahibi}");
                stringBuilder.AppendLine($"Hesap No: {item.HesapNo}");
                stringBuilder.AppendLine(new string('-', 40));
            }

            var telegramMessage = stringBuilder.ToString();
            await telegramBotService.SendLongMessageAsync(TelegramBotConstants.HesaplarChannelId, telegramMessage, 4000, cancellationToken);

            await Task.Delay(200, cancellationToken);

            var telegramResponse = await telegramBotService.SendMessageAsync(TelegramBotConstants.HesaplarChannelId, $"Toplam kayıt: {tedarikciHavaleBankList.Count()}");

            _logger.LogInformation("DoWorkAsync çalıştı. Zaman: {time}", DateTimeOffset.Now);
        }
    }
}
