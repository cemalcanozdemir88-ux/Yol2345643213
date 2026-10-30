# Seq Merkezi Loglama ve Gözlemleme Planı

Bu doküman, farklı Windows sunucularında çalışan .NET API uygulamalarının ayrı bir Windows sunucusunda çalışacak merkezi Seq sistemine güvenli ve dayanıklı biçimde log göndermesi için hazırlanmıştır.

Bu dosya bir uygulama planıdır. Kod değişiklikleri Agent modunda, bu dokümandaki kurallara göre yapılacaktır.

## 1. Hedef Mimari

```text
API Sunucusu 1 ─┐
API Sunucusu 2 ─┼── HTTPS ──> Merkezi Seq Sunucusu
API Sunucusu 3 ─┘                 ├── Seq web paneli
                                  ├── 3 günlük retention
                                  └── Seq veri deposu
```

- Her API kendi Windows sunucusunda çalışmaya devam eder.
- Seq yalnızca ayrı bir merkezi Windows sunucusuna kurulur.
- Uygulama sunucularına Seq kurulmaz.
- API'ler logları HTTP/HTTPS üzerinden merkezi Seq'e gönderir.
- Her uygulama logu `Application` property değeriyle ayrılır.
- Seq bağlantısı uygulamanın çalışması için zorunlu bağımlılık olmayacaktır.

## 2. Merkezi Seq Sunucusu Kurulumu

### 2.1 Sunucu

- Ayrı bir Windows Server kullanılacak.
- Sabit iç ağ IP'si veya DNS adı belirlenecek.
- Yeterli disk alanı olan ayrı bir veri diski tercih edilecek.
- Seq Windows servisi olarak kurulacak.
- Yönetim paneli yalnızca yetkili yöneticilere açılacak.
- Mümkünse HTTPS kullanılacak.
- Seq internete doğrudan açık bırakılmayacak.

Örnek adres:

```text
https://seq.company.internal
```

### 2.2 Firewall

Seq sunucusunun firewall'ında log ingestion erişimi yalnızca izin verilen uygulama sunucularının sabit IP adreslerine açılacak.

- Uygulama sunucularından Seq sunucusuna outbound HTTPS erişimi verilecek.
- Seq yönetim paneli için erişim yalnızca yönetim ağı veya VPN üzerinden verilecek.
- Gereksiz inbound portlar açılmayacak.
- Her uygulama sunucusundan DNS, TCP/HTTPS ve TLS bağlantısı ayrı ayrı test edilecek.

### 2.3 API Key

Her uygulama veya proje için ayrı Seq API key oluşturulacak:

- `TedarikciHizmet.Api`
- `BaskaUygulama.Api`
- `Odeme.Api`

API key:

- Kaynak koda yazılmayacak.
- `appsettings.json` içine production secret olarak commit edilmeyecek.
- İlgili uygulama sunucusunda environment variable, Windows servis environment ayarı veya güvenli secret yönetimiyle tutulacak.
- Projeler arasında aynı key paylaşılmayacak.
- Gerektiğinde ayrı ayrı iptal edilip yenilenebilecek.

### 2.4 Seq retention ve veri yönetimi

- Seq retention policy son 3 takvim günü için ayarlanacak.
- 3 günden eski event'ler Seq tarafından otomatik temizlenecek.
- Manuel log silme gerekmeyecek.
- Seq veri klasörü için disk kapasitesi izlenecek.
- Seq veri klasörü ve yapılandırması için yedekleme/geri yükleme prosedürü belirlenecek.
- Request body logları disk kullanımını artırabileceğinden boyut limitleri zorunlu olacak.

Seq için ayrıca SQL Server, PostgreSQL veya MySQL kurulması gerekmez. Seq kendi dahili veri deposunu kullanır. Veri klasörünün gerçek konumu kurulum ve servis ayarlarında doğrulanmalıdır; varsayılan konuma körü körüne güvenilmemelidir.

## 3. Uygulama Projesine Eklenecek Paketler

Her API projesinde mevcut Serilog altyapısı korunarak aşağıdaki paketler değerlendirilecek ve uygun sürümleriyle eklenecek:

- `Serilog.Sinks.Seq`
- `Serilog.Sinks.Async`

`Serilog.Sinks.Async`, Seq'e gönderimi request thread'inden ayırmak için kullanılacak. Durable disk buffer desteği kullanılan sink/sürüm için ayrıca doğrulanacak; yalnızca memory queue kullanılıyorsa süreç kapanmasında henüz gönderilmemiş event'lerin kaybolabileceği dokümante edilecek.

## 4. Seq Erişilemezlik ve Uygulama Dayanıklılığı

Bu gereksinim zorunludur:

> Seq kapalı, erişilemez, timeout veren veya hata döndüren bir durumda API request'i normal çalışmalı; uygulama çökmemeli ve logging exception'ı controller/middleware akışına sızmamalıdır.

Uygulanacak yaklaşım:

- Seq sink non-blocking/asenkron yapılandırılacak.
- Seq ağı request pipeline'ını bekletmeyecek.
- Timeout ve retry davranışı sınırlı olacak; sonsuz retry yapılmayacak.
- Queue dolduğunda uygulama request'leri bloklanmayacak.
- Mümkünse uygulama sunucusunda durable disk buffer kullanılacak.
- Seq geri geldiğinde buffer'daki event'ler gönderilecek.
- Uygulama sunucusunda sınırlı ve retention'lı rolling file fallback tutulacak.
- Fallback logları Seq'in yerine geçmez; Seq tekrar erişilebilir olana kadar geçici yedektir.
- Logging alt sistemindeki hatalar uygulama exception'ına dönüştürülmeyecek.
- Serilog `SelfLog` tanı kayıtları ayrı, sınırlı ve kontrollü bir hedefe yazılacak.

Test edilmesi gereken durumlar:

- Seq servisinin kapatılması
- DNS çözümleme hatası
- TLS sertifika hatası
- Ağ timeout'u
- Seq HTTP 4xx/5xx cevabı
- Async queue'nun dolması
- Durable buffer diskinin dolması
- Yerel fallback diskinin yazılamaması
- Seq'in tekrar açılması ve bekleyen event'lerin gönderilmesi

## 5. Değiştirilecek Dosyalar

### 5.1 `TedarikciHizmet.Api/TedarikciHizmet.Api.csproj`

- `Serilog.Sinks.Seq` eklenecek.
- `Serilog.Sinks.Async` eklenecek.
- Mevcut paket sürümleriyle uyumlu sürümler seçilecek.
- Gereksiz logging framework'leri eklenmeyecek.

### 5.2 `TedarikciHizmet.Api/Program.cs`

- Serilog uygulama başlangıcında yalnızca bir kez yapılandırılacak.
- `ReadFrom.Configuration(...)` kullanılacak.
- `Application` property'si tüm event'lere eklenecek.
- Seq sink merkezi endpoint ve API key configuration'dan okuyacak.
- API key kaynak kodda tutulmayacak.
- Async/non-blocking gönderim yapılandırılacak.
- Mümkünse durable buffer ayarlanacak.
- Yerel rolling file fallback yapılandırılacak.
- `UseSerilogRequestLogging()` ile özel request/response middleware'in aynı bilgileri gereksiz yere iki kez üretmesi engellenecek.
- Middleware sırası exception'ların yakalanacağı ve TraceId'nin korunacağı şekilde test edilecek.

### 5.3 `TedarikciHizmet.Api/appsettings.json`

Aşağıdaki ayarlar configuration modeline dönüştürülebilecek şekilde tanımlanacak:

- Serilog minimum seviyeleri
- Microsoft framework log seviyeleri
- `Application` adı
- Merkezi Seq URL'i
- Seq API key için environment variable placeholder veya güvenli configuration kaynağı
- Async queue ayarları
- Durable buffer ayarları
- Yerel fallback dosya yolu
- Yerel dosya retention ve boyut sınırı
- Request/response log seviyesi
- Body/header allowlist'i
- Hassas alan listesi
- Request ve response boyut limitleri

Production secret değerleri düz metin olarak commit edilmeyecek.

### 5.4 `TedarikciHizmet.Api/appsettings.Development.json`

- Geliştirme ortamının Seq adresi ayrı tanımlanabilecek.
- Production API key kullanılmayacak.
- Daha ayrıntılı log seviyesi kullanılabilecek.
- Geliştirme ortamında body loglama yalnızca güvenli test endpoint'lerinde açılacak.

### 5.5 `TedarikciHizmet.Api/Infrastructure/ErrorHandlingMiddleware.cs`

- `Log.Logger = new LoggerConfiguration()` kaldırılacak.
- `ILogger<ErrorHandlingMiddleware>` kullanılacak.
- Exception structured log olarak yazılacak.
- HTTP method, path, status code ve TraceId event property'leriyle gönderilecek.
- Gerçek `exception.Message` ve `InnerException.Message` istemciye dönülmeyecek.
- İstemciye genel hata mesajı ve TraceId/correlation bilgisi döndürülecek.
- Seq veya başka bir sink hatası bu middleware'in yeni exception üretmesine neden olmayacak.
- Response zaten başlamışsa tekrar response yazma durumu güvenli ele alınacak.

### 5.6 `TedarikciHizmet.Api/Infrastructure/RequestResponseLoggingMiddleware.cs`

- Global `Log.Logger` yeniden oluşturulmayacak.
- `ILogger<RequestResponseLoggingMiddleware>` kullanılacak.
- Request ve response structured property'lerle loglanacak.
- En az şu bilgiler ilişkilendirilecek:
  - TraceId/correlation ID
  - HTTP method
  - Path ve güvenli query bilgisi
  - Client IP
  - Status code
  - Elapsed time
  - Request/response boyutu
  - Content type
- Request body ve ayrıntılı header production'da yalnızca endpoint allowlist için loglanacak.
- Allowlist dışındaki endpoint'lerde metadata-only davranışı kullanılacak.
- Allowlist içindeki body/header logları da redaction ve boyut sınırlarına tabi olacak.
- `Authorization`, `Cookie`, `Set-Cookie`, API key, proxy credentials ve token içeren header'lar maskelenecek.
- JSON body içinde aşağıdaki gibi hassas alanlar maskelenecek:
  - `password`
  - `currentPassword`
  - `newPassword`
  - `token`
  - `refreshToken`
  - `secret`
  - `cardNumber`
  - `cvv`
- Ham body yalnızca açıkça izin verilen güvenli endpoint/content type için kullanılacak.
- Request ve response için ayrı maksimum boyut limitleri uygulanacak.
- Limit aşılırsa tam içerik yerine sınırlı preview ve `Truncated = true` bilgisi yazılacak.
- Multipart upload, dosya ve binary içerikler loglanmayacak.
- `Content-Length` null veya chunked request güvenli ele alınacak.
- Response body capture yalnızca küçük, text tabanlı ve izin verilen response'larda yapılacak.
- `try/finally` ile response stream'in eski haline dönmesi garanti edilecek.
- Downstream exception durumunda status ve elapsed bilgisi kaybolmayacak.
- Middleware logging hatası request'in başarısız olmasına neden olmayacak.

## 6. Configuration Tasarımı

Body/header ayrıntı seviyesi configuration üzerinden yönetilecek. Önerilen seviyeler:

- `None`: body/header ayrıntısı yok, yalnızca temel metadata
- `Metadata`: boyut/content type gibi metadata
- `RedactedBody`: allowlist içindeki endpoint'lerde redacted body ve izin verilen header'lar

Örnek mantıksal ayarlar:

```json
{
  "Observability": {
    "ApplicationName": "TedarikciHizmet.Api",
    "RequestResponse": {
      "DefaultLevel": "Metadata",
      "AllowedPaths": [
        "/api/ornek-inceleme-endpoint"
      ],
      "MaxRequestBodyBytes": 32768,
      "MaxResponseBodyBytes": 32768,
      "MaxHeaderValueBytes": 4096
    }
  }
}
```

Gerçek production URL'i, API key ve secret değerleri bu dosyaya yazılmayacak. Örnek endpoint uygulamanın gerçek endpoint'leriyle değiştirilecek.

## 7. Seq'te Proje Bazlı İzleme

Her uygulama tüm loglarını aynı Seq sunucusuna gönderebilir. Her event'te en azından şu property bulunacak:

```text
Application = TedarikciHizmet.Api
```

Seq üzerinde şu filtreler kullanılabilecek:

- Uygulama adına göre
- Log seviyesine göre
- HTTP status code'a göre
- TraceId'ye göre
- Belirli endpoint'e göre
- Exception türüne göre
- Belirli sunucu veya environment'a göre

Request, response ve exception aynı TraceId ile ilişkilendirilecek.

## 8. Retention ve Yerel Fallback

- Merkezi Seq: 3 takvim günü retention.
- Yerel fallback: günlük rolling file, sınırlı dosya sayısı ve/veya boyut retention.
- Eski mevcut `logs/error/log-*.txt` dosyaları yeni yapılandırma tarafından geriye dönük otomatik temizlenmeyebilir; geçişte bir defalık operasyonel temizlik planlanmalı.
- Yerel dosya fallback'inin de sınırsız büyümesine izin verilmeyecek.
- Request body ayrıntısı disk kullanımını artıracağından allowlist ve boyut sınırı korunacak.

## 9. Güvenlik

- Loglara parola, JWT signing key, OTP secret, database connection string, token veya kart güvenlik verisi yazılmayacak.
- Mevcut configuration dosyalarında görülen secret değerleri ayrıca rotasyona alınmalı ve güvenli secret yönetimine taşınmalıdır.
- Seq yönetim paneli internete açık bırakılmamalı.
- Uygulama başına ayrı API key kullanılmalı.
- API key'ler source control'a gönderilmemeli.
- Request body logları kişisel/veri koruma gereksinimleri açısından ayrıca değerlendirilmelidir.
- Seq üzerinde kullanıcı rolleri ve erişim yetkileri sınırlandırılmalıdır.

## 10. Doğrulama Planı

Kod değişikliklerinden sonra:

1. `dotnet restore`
2. `dotnet build`
3. Kontrollü bir 500 hata üretme
4. Seq'te exception ve stack trace kontrolü
5. `Application = 'TedarikciHizmet.Api'` filtresiyle arama
6. Request, response ve exception'ın aynı TraceId ile bulunduğunu doğrulama
7. Allowlist endpoint'inde redacted body/header logunu doğrulama
8. Allowlist dışındaki endpoint'te metadata-only davranışını doğrulama
9. Authorization, password, token ve secret değerlerinin maskelendiğini doğrulama
10. Büyük body için kesilmiş preview ve `Truncated` bilgisini doğrulama
11. Content-Length olmayan request testi
12. Multipart ve binary request testi
13. Downstream exception testi
14. Seq servisini durdurup API'ye request gönderme
15. API'nin çökmediğini ve response'ların normal kaldığını doğrulama
16. Yerel fallback logunun oluştuğunu doğrulama
17. Seq yeniden açıldığında buffer event'lerinin gönderildiğini doğrulama
18. DNS, TLS, timeout, HTTP hata ve queue doluluğu senaryolarını test etme
19. Uygulama yeniden başlatma testi
20. Seq retention ve disk kullanımını doğrulama

## 11. Uygulanmayacaklar

- Her hata için ayrı `.txt` dosyası oluşturulmayacak.
- Request başına Serilog global logger yeniden yapılandırılmayacak.
- Production'da tüm endpoint'lerin ham request body'si varsayılan olarak loglanmayacak.
- Seq bağlantısı API request'lerini bloklamayacak.
- Seq hatası uygulamayı çökertmeyecek.
- Gereksiz şekilde Grafana, Loki ve Promtail kurulmayacak; bunlar ayrı bir ihtiyaç doğarsa değerlendirilecek.

## 12. Uygulama Sırası

1. Merkezi Seq sunucusunu hazırlama.
2. DNS, HTTPS, firewall ve API key bağlantılarını test etme.
3. Proje paketlerini ve Serilog başlangıç yapılandırmasını güncelleme.
4. ErrorHandlingMiddleware'i structured logging ve güvenli response için güncelleme.
5. RequestResponseLoggingMiddleware'i allowlist, redaction, boyut limitleri ve stream güvenliğiyle güncelleme.
6. Yerel fallback ve async/durable buffer davranışını yapılandırma.
7. Seq erişilememe testlerini çalıştırma.
8. Request/response/body/header ve güvenlik testlerini çalıştırma.
9. Build ve uygulama çalıştırma doğrulamasını tamamlama.
