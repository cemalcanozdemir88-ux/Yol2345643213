/*
  Mutabakat çift kayıt temizleme
  Aynı gün için aynı site / tedarikçiye birden fazla mutabakat satırı varsa
  EN SON kaydedileni bırakır, diğerlerini siler.

  1) Önce sadece 1. ADIM'ı çalıştırıp çift kayıtları görün.
  2) Sonra 2. ADIM'ı çalıştırın. Sonuç doğruysa COMMIT, değilse ROLLBACK yazıp çalıştırın.
*/

------------------------------------------------------------
-- 1. ADIM: Çift kayıtları göster (hiçbir şey silmez)
------------------------------------------------------------
SELECT 'WebSite' AS Tur, WebSiteId AS Id, MutabakatTarihi, COUNT(*) AS Adet
FROM WebSiteMutabakatlari
GROUP BY WebSiteId, MutabakatTarihi
HAVING COUNT(*) > 1
UNION ALL
SELECT 'Tedarikci', TedarikciId, MutabakatTarihi, COUNT(*)
FROM TedarikciMutabakatlari
GROUP BY TedarikciId, MutabakatTarihi
HAVING COUNT(*) > 1
ORDER BY MutabakatTarihi DESC;

------------------------------------------------------------
-- 2. ADIM: Çiftleri sil (en son kaydedilen kalır)
------------------------------------------------------------
BEGIN TRANSACTION;

;WITH Cift AS (
    SELECT ROW_NUMBER() OVER (PARTITION BY WebSiteId, MutabakatTarihi
                              ORDER BY KayitTarihi DESC, Id DESC) AS Sira
    FROM WebSiteMutabakatlari
)
DELETE FROM Cift WHERE Sira > 1;
PRINT CONCAT('WebSiteMutabakatlari silinen: ', @@ROWCOUNT);

;WITH Cift AS (
    SELECT ROW_NUMBER() OVER (PARTITION BY TedarikciId, MutabakatTarihi
                              ORDER BY KayitTarihi DESC, Id DESC) AS Sira
    FROM TedarikciMutabakatlari
)
DELETE FROM Cift WHERE Sira > 1;
PRINT CONCAT('TedarikciMutabakatlari silinen: ', @@ROWCOUNT);

-- Kontrol: aşağıdaki sorgu boş dönmeli
SELECT WebSiteId, MutabakatTarihi, COUNT(*) FROM WebSiteMutabakatlari
GROUP BY WebSiteId, MutabakatTarihi HAVING COUNT(*) > 1;
SELECT TedarikciId, MutabakatTarihi, COUNT(*) FROM TedarikciMutabakatlari
GROUP BY TedarikciId, MutabakatTarihi HAVING COUNT(*) > 1;

-- Her şey doğruysa:   COMMIT;
-- Geri almak için:    ROLLBACK;
