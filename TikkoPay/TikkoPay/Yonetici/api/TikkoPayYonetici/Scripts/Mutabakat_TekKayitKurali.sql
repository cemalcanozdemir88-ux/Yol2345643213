/*
  Mutabakat: "aynı gün için tek kayıt" kuralı (kalıcı çözüm)

  Ne yapar?
    - Mevcut çift kayıtları temizler (her site/tedarikçi + gün için EN SON kaydedileni bırakır).
    - Veritabanına, aynı site/tedarikçi + gün için ikinci bir kaydın eklenmesini
      ENGELLEYEN benzersizlik kuralı (unique index) ekler.

  Nasıl çalıştırılır?
    1) Önce sadece 1. ADIM'ı çalıştırın: çift kayıtları listeler, hiçbir şey değiştirmez.
    2) Sonra 2. ADIM'ın tamamını çalıştırın. Hata olursa otomatik geri alınır.
    3) Bu script ile birlikte yeni kod sunucuya yüklenmelidir (sıra önemli değil).

  Tekrar çalıştırılabilir: kural zaten varsa yeniden oluşturmaz.
*/

------------------------------------------------------------
-- 1. ADIM: Çift kayıtları göster (hiçbir şey değiştirmez)
------------------------------------------------------------
SELECT 'WebSite' AS Tur, WebSiteId AS Id, MutabakatTarihi, COUNT(*) AS Adet,
       MIN(KayitTarihi) AS IlkKayit, MAX(KayitTarihi) AS SonKayit
FROM WebSiteMutabakatlari
GROUP BY WebSiteId, MutabakatTarihi
HAVING COUNT(*) > 1
UNION ALL
SELECT 'Tedarikci', TedarikciId, MutabakatTarihi, COUNT(*),
       MIN(KayitTarihi), MAX(KayitTarihi)
FROM TedarikciMutabakatlari
GROUP BY TedarikciId, MutabakatTarihi
HAVING COUNT(*) > 1
ORDER BY MutabakatTarihi DESC;
GO

------------------------------------------------------------
-- 2. ADIM: Çiftleri temizle + kuralı ekle
------------------------------------------------------------
SET XACT_ABORT ON;
BEGIN TRANSACTION;

;WITH Cift AS (
    SELECT ROW_NUMBER() OVER (PARTITION BY WebSiteId, MutabakatTarihi
                              ORDER BY KayitTarihi DESC, Id DESC) AS Sira
    FROM WebSiteMutabakatlari WITH (TABLOCKX, HOLDLOCK)
)
DELETE FROM Cift WHERE Sira > 1;
PRINT CONCAT('WebSiteMutabakatlari silinen cift kayit: ', @@ROWCOUNT);

;WITH Cift AS (
    SELECT ROW_NUMBER() OVER (PARTITION BY TedarikciId, MutabakatTarihi
                              ORDER BY KayitTarihi DESC, Id DESC) AS Sira
    FROM TedarikciMutabakatlari WITH (TABLOCKX, HOLDLOCK)
)
DELETE FROM Cift WHERE Sira > 1;
PRINT CONCAT('TedarikciMutabakatlari silinen cift kayit: ', @@ROWCOUNT);

IF NOT EXISTS (SELECT 1 FROM sys.indexes
               WHERE name = 'UX_WebSiteMutabakatlari_WebSiteId_MutabakatTarihi'
                 AND object_id = OBJECT_ID('dbo.WebSiteMutabakatlari'))
BEGIN
    CREATE UNIQUE INDEX UX_WebSiteMutabakatlari_WebSiteId_MutabakatTarihi
        ON dbo.WebSiteMutabakatlari (WebSiteId, MutabakatTarihi);
    PRINT 'WebSiteMutabakatlari tek kayit kurali eklendi.';
END

IF NOT EXISTS (SELECT 1 FROM sys.indexes
               WHERE name = 'UX_TedarikciMutabakatlari_TedarikciId_MutabakatTarihi'
                 AND object_id = OBJECT_ID('dbo.TedarikciMutabakatlari'))
BEGIN
    CREATE UNIQUE INDEX UX_TedarikciMutabakatlari_TedarikciId_MutabakatTarihi
        ON dbo.TedarikciMutabakatlari (TedarikciId, MutabakatTarihi);
    PRINT 'TedarikciMutabakatlari tek kayit kurali eklendi.';
END

COMMIT TRANSACTION;
GO

-- Kontrol: iki sorgu da BOŞ dönmeli
SELECT WebSiteId, MutabakatTarihi, COUNT(*) FROM WebSiteMutabakatlari
GROUP BY WebSiteId, MutabakatTarihi HAVING COUNT(*) > 1;
SELECT TedarikciId, MutabakatTarihi, COUNT(*) FROM TedarikciMutabakatlari
GROUP BY TedarikciId, MutabakatTarihi HAVING COUNT(*) > 1;
