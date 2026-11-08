/*=====================================================================
  Web sitesine banka havale hesabı bağlama
  ---------------------------------------------------------------------
  Kural (bir web sitesi + bir tedarikçi için):
    - Siteye o tedarikçinin HESABI bağlandıysa  -> sadece bağlanan hesaplar kullanılır
      (tedarikçi ayrıca bağlı olmasa da; hesap bağlamak tedarikçiyi de bağlar)
    - Sadece TEDARİKÇİ bağlıysa                 -> tedarikçinin tüm hesapları kullanılır (eski davranış)
    - Hiçbiri bağlı değilse                     -> tedarikçi bu sitede kullanılmaz
  Hesabın kendi koşulları (min/max tutar, adet, toplam) ve tedarikçinin
  koşulları (puan, bakiye limiti, min yatırım) her durumda geçerlidir.

  Bu script SADECE YENİ nesneler oluşturur, mevcut hiçbir şeyi değiştirmez:
    - Tablo    : WebSiteHavaleBankalari
    - Fonksiyon: GetWebSiteUygunHavaleBankalari
    - SP       : GetAvailableHavaleBankItemDtoWithWebSiteV2
  Canlıdaki eski SP (GetAvailableHavaleBankItemDtoWithWebSite) olduğu gibi kalır.
  Yeni SP'yi sadece yeni Transactions kodu çağırır; eski kod çalıştığı sürece
  bu script'i canlıda çalıştırmak hiçbir işlemi etkilemez.

  Çalıştırma sırası: 1 -> 2 -> 3. Geri almak için en alttaki GERİ ALMA bölümü.
=====================================================================*/

/*---------------------------------------------------------------------
  1) Tablo
---------------------------------------------------------------------*/
IF OBJECT_ID('dbo.WebSiteHavaleBankalari', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.WebSiteHavaleBankalari (
        Id                      UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_WebSiteHavaleBankalari PRIMARY KEY,
        WebSiteId               UNIQUEIDENTIFIER NOT NULL,
        HavaleBankaId           UNIQUEIDENTIFIER NOT NULL,
        SilindiMi               BIT              NOT NULL CONSTRAINT DF_WebSiteHavaleBankalari_SilindiMi DEFAULT (0),
        KayitTarihi             DATETIME         NOT NULL,
        KaydedenKullaniciId     UNIQUEIDENTIFIER NULL,
        GuncellemeTarihi        DATETIME         NULL,
        GuncelleyenKullaniciId  UNIQUEIDENTIFIER NULL,
        CONSTRAINT FK_WebSiteHavaleBankalari_WebSiteleri      FOREIGN KEY (WebSiteId)     REFERENCES dbo.WebSiteleri(Id),
        CONSTRAINT FK_WebSiteHavaleBankalari_HavaleBankalari  FOREIGN KEY (HavaleBankaId) REFERENCES dbo.HavaleBankalari(Id)
    );

    CREATE INDEX IX_WebSiteHavaleBankalari_WebSiteId
        ON dbo.WebSiteHavaleBankalari (WebSiteId, SilindiMi) INCLUDE (HavaleBankaId);
END
GO

/*---------------------------------------------------------------------
  2) Bir web sitesinde kullanılabilecek hesaplar (sadece bağlantı kuralı;
     tutar/limit koşulları SP içinde kontrol edilir)
---------------------------------------------------------------------*/
CREATE OR ALTER FUNCTION dbo.GetWebSiteUygunHavaleBankalari (@webSiteId UNIQUEIDENTIFIER)
RETURNS TABLE
AS
RETURN
(
    SELECT hb.Id AS HavaleBankaId, hb.TedarikciId
    FROM HavaleBankalari hb
    WHERE hb.SilindiMi = 0
      AND (
            -- a) Hesap siteye doğrudan bağlı
            EXISTS (
                SELECT 1 FROM WebSiteHavaleBankalari wh
                WHERE wh.WebSiteId = @webSiteId
                  AND wh.HavaleBankaId = hb.Id
                  AND wh.SilindiMi = 0
            )
            OR
            -- b) Tedarikçi siteye bağlı ve bu tedarikçi için siteye hiç hesap bağlanmamış
            (
                EXISTS (
                    SELECT 1 FROM WebSiteTedarikcileri wst
                    WHERE wst.WebSiteId = @webSiteId
                      AND wst.TedarikciId = hb.TedarikciId
                      AND wst.SilindiMi = 0
                )
                AND NOT EXISTS (
                    SELECT 1
                    FROM WebSiteHavaleBankalari wh
                    INNER JOIN HavaleBankalari hb2 ON hb2.Id = wh.HavaleBankaId
                    WHERE wh.WebSiteId = @webSiteId
                      AND wh.SilindiMi = 0
                      AND hb2.SilindiMi = 0
                      AND hb2.TedarikciId = hb.TedarikciId
                )
            )
          )
);
GO

/*---------------------------------------------------------------------
  3) Hesap seçen SP (yeni, ayrı isimle; eski SP'ye dokunulmaz)
     Eski SP'den farkları:
       - WebSiteTedarikcileri yerine GetWebSiteUygunHavaleBankalari kullanılır
       - 2. adımda (hesap döndürme) 1. adımdaki koşulların aynısı uygulanır
         (adet/toplam limiti ve banka filtresi), böylece seçilen tedarikçinin
         koşulu sağlamayan bir hesabı dönmez
---------------------------------------------------------------------*/
CREATE OR ALTER PROCEDURE [dbo].[GetAvailableHavaleBankItemDtoWithWebSiteV2]
    @amount DECIMAL(13,4),
    @webSiteId UNIQUEIDENTIFIER,
    @bankaId NVARCHAR(50) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY

        /*---------------------------------------------------------
        0) Website bazlı HesapNoGosterimSayisi reset
        ---------------------------------------------------------*/
        IF NOT EXISTS (
            SELECT 1
            FROM Tedarikciler t
            WHERE
                t.Id IN (SELECT u.TedarikciId FROM dbo.GetWebSiteUygunHavaleBankalari(@webSiteId) u)
                AND t.BankaHavale = 1
                AND t.Durum = 1
                AND t.SilindiMi = 0
                AND t.YatirimPuani > 0
                AND t.HesapNoGosterimSayisi <= t.YatirimPuani
                AND @amount >= t.MinYatirimTutari
                AND EXISTS (
                    SELECT 1
                    FROM dbo.GetWebSiteUygunHavaleBankalari(@webSiteId) u
                    INNER JOIN HavaleBankalari hb ON hb.Id = u.HavaleBankaId
                    WHERE
                        hb.TedarikciId = t.Id
                        AND hb.HesapMinTutar <= @amount
                        AND hb.HesapMaxTutar >= @amount
                        AND hb.OnaylananYatirimAdedi < hb.MaxYatirimAdedi
                        AND hb.OnaylananYatirimToplami < hb.MaxYatirimToplami
                        AND hb.Durum = 1
                        AND hb.SilindiMi = 0
                )
        )
        BEGIN
            UPDATE t WITH (ROWLOCK, UPDLOCK)
            SET HesapNoGosterimSayisi = 0
            FROM Tedarikciler t
            WHERE
                t.Id IN (SELECT u.TedarikciId FROM dbo.GetWebSiteUygunHavaleBankalari(@webSiteId) u)
                AND t.BankaHavale = 1
                AND t.Durum = 1
                AND t.SilindiMi = 0;
        END;

        /*---------------------------------------------------------
        1) Website bazlı atomik tedarikçi seçimi
        ---------------------------------------------------------*/
        DECLARE @TempResult TABLE (
            TedarikciId UNIQUEIDENTIFIER
        );

        ;WITH Candidate AS (
            SELECT TOP 1
                t.Id AS TedarikciId
            FROM dbo.GetWebSiteUygunHavaleBankalari(@webSiteId) u
            INNER JOIN Tedarikciler t WITH (ROWLOCK, UPDLOCK) ON t.Id = u.TedarikciId
            INNER JOIN HavaleBankalari hb WITH (NOLOCK) ON hb.Id = u.HavaleBankaId
            CROSS APPLY dbo.GetTedarikciBakiyeWithBekleyen_iTVF(t.Id) AS bakiye
            WHERE
                @amount >= t.MinYatirimTutari
                AND hb.HesapMinTutar <= @amount
                AND hb.HesapMaxTutar >= @amount
                AND hb.OnaylananYatirimAdedi < hb.MaxYatirimAdedi
                AND hb.OnaylananYatirimToplami < hb.MaxYatirimToplami
                AND t.YatirimPuani > 0
                AND t.BankaHavale = 1
                AND t.Durum = 1
                AND t.SilindiMi = 0
                AND hb.Durum = 1
                AND hb.SilindiMi = 0
                AND bakiye.HesaplananBakiye < t.BakiyeLimitor
                AND t.YatirimPuani >= t.HesapNoGosterimSayisi
                AND (@bankaId IS NULL OR hb.BankaId = @bankaId)
            ORDER BY
                t.HesapNoGosterimSayisi,
                t.YatirimPuani DESC,
                NEWID()
        )
        UPDATE t
        SET t.HesapNoGosterimSayisi = t.HesapNoGosterimSayisi + 1
        OUTPUT inserted.Id INTO @TempResult(TedarikciId)
        FROM Tedarikciler t
        INNER JOIN Candidate c ON t.Id = c.TedarikciId;

        IF NOT EXISTS (SELECT 1 FROM @TempResult)
            RETURN;

        DECLARE @tedarikciId UNIQUEIDENTIFIER;
        SELECT TOP 1 @tedarikciId = TedarikciId FROM @TempResult;

        /*---------------------------------------------------------
        2) Seçilen tedarikçi için, siteye uygun ve koşulu sağlayan hesabı döndür
        ---------------------------------------------------------*/
        SELECT TOP 1
            hb.Id,
            hb.TedarikciId,
            hb.HesapSahibi,
            hb.HesapNo
        FROM dbo.GetWebSiteUygunHavaleBankalari(@webSiteId) u
        INNER JOIN HavaleBankalari hb WITH (NOLOCK) ON hb.Id = u.HavaleBankaId
        WHERE
            hb.TedarikciId = @tedarikciId
            AND hb.HesapMinTutar <= @amount
            AND hb.HesapMaxTutar >= @amount
            AND hb.OnaylananYatirimAdedi < hb.MaxYatirimAdedi
            AND hb.OnaylananYatirimToplami < hb.MaxYatirimToplami
            AND hb.Durum = 1
            AND hb.SilindiMi = 0
            AND (@bankaId IS NULL OR hb.BankaId = @bankaId)
        ORDER BY NEWID();

    END TRY
    BEGIN CATCH
        DECLARE @ErrMsg NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrNo INT = ERROR_NUMBER();
        RAISERROR(
            'GetAvailableHavaleBankItemDto failed: %s (Error %d)',
            16, 1, @ErrMsg, @ErrNo
        );
    END CATCH
END
GO


/*=====================================================================
  GERİ ALMA (gerekirse)
  Canlıdaki eski SP (GetAvailableHavaleBankItemDtoWithWebSite) bu script
  tarafından DEĞİŞTİRİLMEZ. Geri dönmek için eski Transactions kodunu
  yayınlamanız yeterlidir. Yeni nesneleri tamamen kaldırmak isterseniz:
=====================================================================*/
/*
DROP PROCEDURE IF EXISTS dbo.GetAvailableHavaleBankItemDtoWithWebSiteV2;
DROP FUNCTION  IF EXISTS dbo.GetWebSiteUygunHavaleBankalari;
DROP TABLE     IF EXISTS dbo.WebSiteHavaleBankalari;
*/
