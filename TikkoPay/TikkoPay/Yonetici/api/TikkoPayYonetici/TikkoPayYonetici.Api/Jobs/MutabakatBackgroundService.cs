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
                catch (Exception ex)
                {
                    // Beklenmeyen bir hata job'u tamamen durdurmasın; ertesi gece yine çalışsın.
                    WriteMutabakatLog($"Mutabakat job'u hata verdi: {ex}");
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
            List<WebSiteleriListDto> webSiteleri;
            using (var listScope = _serviceProvider.CreateScope())
            {
                var webSiteleriService = listScope.ServiceProvider.GetRequiredService<IWebSiteleriService>();
                webSiteleri = (await webSiteleriService.GetAllAsync(null)).Result?.ToList() ?? new();
            }

            // Tarih döngüden önce bir kez belirlenir; tüm siteler aynı gün için hesaplanır.
            DateOnly mutabakatTarihi = DateOnly.FromDateTime(DateTime.Now.AddDays(-1));

            foreach (var webSite in webSiteleri)
            {
                // Bir sitede hata olursa kaybolmasın diye 3 kez denenir.
                var hesaplandi = false;
                for (var deneme = 1; deneme <= 3; deneme++)
                {
                    try
                    {
                        // Her site kendi temiz veritabanı bağlantısıyla hesaplanır; birinin hatası sıradakine bulaşmaz.
                        using var scope = _serviceProvider.CreateScope();
                        var webSiteMutabakatService = scope.ServiceProvider.GetRequiredService<IWebSiteMutabakatService>();

                        // Devir, Calculate içinde bir önceki günün kaydından alınır.
                        var sonuc = await webSiteMutabakatService.Calculate(webSite.Id, 0, mutabakatTarihi);
                        if (sonuc.Success)
                        {
                            hesaplandi = true;
                            break;
                        }

                        WriteMutabakatLog($"WebSite: {webSite.Baslik} ({webSite.Id}) - Tarih: {mutabakatTarihi:yyyy-MM-dd} - Deneme {deneme} başarısız: {sonuc.Message}");
                    }
                    catch (Exception ex)
                    {
                        WriteMutabakatLog($"WebSite: {webSite.Baslik} ({webSite.Id}) - Tarih: {mutabakatTarihi:yyyy-MM-dd} - Deneme {deneme} HATA: {ex}");
                    }

                    if (deneme < 3)
                        await Task.Delay(TimeSpan.FromSeconds(10));
                }

                if (!hesaplandi)
                    continue;

                // Telegram hatası hesaplamayı tekrarlatmasın diye ayrı tutulur (mesaj iki kez gitmesin).
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var webSiteMutabakatService = scope.ServiceProvider.GetRequiredService<IWebSiteMutabakatService>();
                    var telegramBotService = scope.ServiceProvider.GetRequiredService<ITelegramBotService>();
                    await SendTelegramMessageForWebSite(webSite, mutabakatTarihi, webSiteMutabakatService, telegramBotService);
                }
                catch (Exception ex)
                {
                    WriteMutabakatLog($"WebSite: {webSite.Baslik} ({webSite.Id}) - Tarih: {mutabakatTarihi:yyyy-MM-dd} - Telegram mesajı gönderilemedi: {ex}");
                }
            }
        }

        private async Task CalculateTedarikciMutabakat()
        {
            List<Core.DTOs.Tedarikciler.TedarikcilerListDto> tedarikciler;
            using (var listScope = _serviceProvider.CreateScope())
            {
                var tedarikcilerService = listScope.ServiceProvider.GetRequiredService<ITedarikciService>();
                tedarikciler = (await tedarikcilerService.GetAll()).Result ?? new();
            }

            // Tarih döngüden önce bir kez belirlenir; tüm tedarikçiler aynı gün için hesaplanır.
            DateOnly mutabakatTarihi = DateOnly.FromDateTime(DateTime.Now.AddDays(-1));

            foreach (var tedarikci in tedarikciler)
            {
                // Bir tedarikçide hata olursa kaybolmasın diye 3 kez denenir.
                for (var deneme = 1; deneme <= 3; deneme++)
                {
                    try
                    {
                        // Her tedarikçi kendi temiz veritabanı bağlantısıyla hesaplanır; birinin hatası
                        // (yarım kalmış kayıt vb.) sıradaki tedarikçiye bulaşmaz.
                        using var scope = _serviceProvider.CreateScope();
                        var tedarikciMutabakatService = scope.ServiceProvider.GetRequiredService<ITedarikciMutabakatService>();

                        // Devir, Calculate içinde bir önceki günün kaydından alınır.
                        var sonuc = await tedarikciMutabakatService.Calculate(tedarikci.Id, 0, mutabakatTarihi);
                        if (sonuc.Success)
                            break;

                        WriteMutabakatLog($"Tedarikçi: {tedarikci.Unvan} ({tedarikci.Id}) - Tarih: {mutabakatTarihi:yyyy-MM-dd} - Deneme {deneme} başarısız: {sonuc.Message}");
                    }
                    catch (Exception ex)
                    {
                        WriteMutabakatLog($"Tedarikçi: {tedarikci.Unvan} ({tedarikci.Id}) - Tarih: {mutabakatTarihi:yyyy-MM-dd} - Deneme {deneme} HATA: {ex}");
                    }

                    if (deneme < 3)
                        await Task.Delay(TimeSpan.FromSeconds(10));
                }
            }
        }

        // Hatalar eskiden sessizce yutuluyordu; artık logs/mutabakat klasörüne günlük dosyaya yazılır.
        private void WriteMutabakatLog(string message)
        {
            try
            {
                var dir = Path.Combine(AppContext.BaseDirectory, "logs", "mutabakat");
                Directory.CreateDirectory(dir);
                File.AppendAllText(Path.Combine(dir, $"{DateTime.Now:yyyy-MM-dd}.txt"), $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} {message}{Environment.NewLine}{Environment.NewLine}", Encoding.UTF8);
            }
            catch
            {
            }

            _logger.LogError("{Message}", message);
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
