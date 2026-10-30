using System.Globalization;
using System.Text;
using TikkoPayYonetici.Core.DTOs.WebSiteleri;
using TikkoPayYonetici.Services.Interfaces;

namespace TikkoPayYonetici.Api.Jobs
{
    public class MutabakatBackgroundService : BackgroundService
    {
        private readonly TimeSpan _executionTime = new TimeSpan(0, 0, 10);

        private readonly ILogger<MutabakatBackgroundService> _logger;
        private readonly IServiceProvider _serviceProvider;

        public MutabakatBackgroundService(IServiceProvider serviceProvider, ILogger<MutabakatBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var now = DateTime.Now;
                var nextRunTime = now.Date.Add(_executionTime);

                if (now > nextRunTime)
                {
                    nextRunTime = nextRunTime.AddDays(1);
                }

                var delay = nextRunTime - now;
                _logger.LogInformation("Bir sonraki çalışma zamanı: {time}. Beklenen süre: {delay}", nextRunTime, delay);

                try
                {
                    await Task.Delay(delay, stoppingToken);

                    await DoWork();
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
        }

        private async Task DoWork()
        {
            await CalculateTedarikciMutabakat();
            await CalculateWebSiteMutabakat();
        }

        private async Task CalculateWebSiteMutabakat()
        {
            using var scope = _serviceProvider.CreateScope();

            var _webSiteMutabakatService = scope.ServiceProvider.GetRequiredService<IWebSiteMutabakatService>();
            var _webSiteleri = scope.ServiceProvider.GetRequiredService<IWebSiteleriService>();
            var _telegramBotService = scope.ServiceProvider.GetRequiredService<ITelegramBotService>();

            var webSiteleri = await _webSiteleri.GetAllAsync(null);
            foreach (var webSite in webSiteleri.Result)
            {
                try
                {
                    var now = DateTime.Now;
                    DateOnly mutabakatTarihi = DateOnly.FromDateTime(now.AddDays(-1));
                    DateOnly mutabakatTarihiPrevious = DateOnly.FromDateTime(now.AddDays(-2));

                    decimal devir = 0;
                    var previousMutabakat = await _webSiteMutabakatService.GetAllDtoAsync(webSite.Id, mutabakatTarihiPrevious);
                    if (previousMutabakat.Success && previousMutabakat.Result.Any())
                    {
                        devir = previousMutabakat.Result.First().Kasa;
                    }

                    await _webSiteMutabakatService.Calculate(webSite.Id, devir, mutabakatTarihi);

                    await SendTelegramMessageForWebSite(webSite, mutabakatTarihi, _webSiteMutabakatService, _telegramBotService);
                }
                catch 
                {

                }
            }
        }

        private async Task CalculateTedarikciMutabakat()
        {
            using var scope = _serviceProvider.CreateScope();

            var _tedarikciMutabakatService = scope.ServiceProvider.GetRequiredService<ITedarikciMutabakatService>();
            var _tedarikcilerService = scope.ServiceProvider.GetRequiredService<ITedarikciService>();

            var tedarikciler = await _tedarikcilerService.GetAll();
            foreach (var tedarikci in tedarikciler.Result)
            {
                try
                {
                    var now = DateTime.Now;
                    DateOnly mutabakatTarihi = DateOnly.FromDateTime(now.AddDays(-1));
                    DateOnly mutabakatTarihiPrevious = DateOnly.FromDateTime(now.AddDays(-2));

                    decimal devir = 0;
                    var previousMutabakat = await _tedarikciMutabakatService.GetAllDtoAsync(tedarikci.Id, mutabakatTarihiPrevious);
                    if (previousMutabakat.Success && previousMutabakat.Result.Any())
                    {
                        devir = previousMutabakat.Result.First().Kasa;
                    }

                    await _tedarikciMutabakatService.Calculate(tedarikci.Id, devir, mutabakatTarihi);
                }
                catch
                {
                }
            }
        }

        private async Task SendTelegramMessageForWebSite(WebSiteleriListDto webSite, DateOnly mutabakatTarihi, IWebSiteMutabakatService webSiteMutabakatService, ITelegramBotService telegramBotService)
        {
            if (string.IsNullOrEmpty(webSite.TelegramChannelId))
            {
                return;
            }

            var siteMutabakat = await webSiteMutabakatService.GetAllDtoAsync(webSite.Id, mutabakatTarihi);
            if (siteMutabakat.Success && siteMutabakat.Result.Any())
            {
                var siteMutabakatItem = siteMutabakat.Result.FirstOrDefault()!;

                CultureInfo tr = new CultureInfo("tr-TR");

                StringBuilder sbMessage = new StringBuilder();
                sbMessage.AppendLine($"{webSite.Baslik} Mutabakat");
                sbMessage.AppendLine($"{mutabakatTarihi.ToString("dd.MM.yyyy")}");
                sbMessage.AppendLine("");
                sbMessage.AppendLine($"Devir : {siteMutabakatItem.Devir.ToString("C", tr)}");
                sbMessage.AppendLine($"Toplam Yatırım : {siteMutabakatItem.ToplamYatirim.ToString("C", tr)}");
                sbMessage.AppendLine($"Toplam Yatırım Komisyonu : {siteMutabakatItem.ToplamYatirimKomisyon.ToString("C", tr)}");
                sbMessage.AppendLine($"Toplam Çekim : {siteMutabakatItem.ToplamCekim.ToString("C", tr)}");
                sbMessage.AppendLine($"Toplam Çekim Komisyonu : {siteMutabakatItem.ToplamCekimKomisyon.ToString("C", tr)}");
                sbMessage.AppendLine($"Alınan Ödeme : {siteMutabakatItem.ToplamGiris.ToString("C", tr)}");
                sbMessage.AppendLine($"Yapılan Ödeme : {siteMutabakatItem.ToplamCikis.ToString("C", tr)}");
                sbMessage.AppendLine($"Yapılan Ödeme Komisyonu : {siteMutabakatItem.ToplamCikisKomisyon.ToString("C", tr)}");
                sbMessage.AppendLine($"Manuel Alınan Ödeme : {siteMutabakatItem.ToplamGirisManual.ToString("C", tr)}");
                sbMessage.AppendLine($"Manuel Yapılan Ödeme : {siteMutabakatItem.ToplamCikisManual.ToString("C", tr)}");
                sbMessage.AppendLine($"Virman (+) : {siteMutabakatItem.ToplamVirmanGiris.ToString("C", tr)}");
                sbMessage.AppendLine($"Virman (-) : {siteMutabakatItem.ToplamVirmanCikis.ToString("C", tr)}");
                sbMessage.AppendLine($"Kasa : {siteMutabakatItem.Kasa.ToString("C", tr)}");

                await telegramBotService.SendLongMessageAsync(webSite.TelegramChannelId!, sbMessage.ToString());
            }
        }
    }
}
