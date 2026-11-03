-- "İptal Edilen Yatırımları Yeniden Gönderebilir" yetkisini ekler.
-- "İptal Edilen Yatırımları Bekleyene Alabilir" yetkisiyle aynı grubun (UstId) altına, hemen arkasına eklenir.
-- Birden fazla kez çalıştırılırsa tekrar eklemez.

IF NOT EXISTS (SELECT 1 FROM Yetkiler WHERE Id = '54809866-1624-41EF-BA78-981D4BC1FB09')
BEGIN
    INSERT INTO Yetkiler (Id, UstId, SiraNumarasi, Baslik, SilindiMi, KayitTarihi)
    SELECT
        '54809866-1624-41EF-BA78-981D4BC1FB09',
        UstId,
        SiraNumarasi + 1,
        N'İptal Edilen Yatırımları Yeniden Gönderebilir',
        0,
        GETDATE()
    FROM Yetkiler
    WHERE Id = 'B5981CAB-1C99-4765-9401-B9E0FAC9A8A1';
END

-- Kontrol: aşağıdaki sorgu 1 satır döndürmeli
SELECT Id, UstId, SiraNumarasi, Baslik FROM Yetkiler WHERE Id = '54809866-1624-41EF-BA78-981D4BC1FB09';
