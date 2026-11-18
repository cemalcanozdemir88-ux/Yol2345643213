-- BluePara tedarikçi entegrasyonu ayarları (SettingsV2).
-- ÇALIŞTIRMADAN ÖNCE: aşağıdaki <...> alanlarını doldurun.
--   1) Yönetici panelinden "BluePara" tedarikçisini oluşturun, Id'sini BlueParaTedarikciId'ye yazın.
--   2) Bu tedarikçiye bir FAST havale banka hesabı (HavaleBankalari) ekleyin, Id'sini BlueParaDefaultFastHavaleBankId'ye yazın.
--      (IBAN her yatırımda BluePara'dan gelir; bu kayıt sadece yatırımın hangi hesaba yazılacağını belirler.)
--   3) API Key ve Provider Key BluePara'nın size verdiği anahtarlardır.
--      (BluePara ileride Merchant Key verirse: Merchant Key'i yazıp Provider Key'i boş bırakın, sistem hash'li yönteme geçer.)
--   4) BlueParaCallbackUrl: Callbacks projesinin canlı adresi + /api/BluePara
-- Birden fazla kez çalıştırılırsa var olan değeri GÜNCELLER, tekrar eklemez.

DECLARE @Ayarlar TABLE ([Key] NVARCHAR(200), [Value] NVARCHAR(MAX));

INSERT INTO @Ayarlar ([Key], [Value]) VALUES
    ('BlueParaTedarikciId',             '<TEDARIKCI_ID>'),
    ('BlueParaDefaultFastHavaleBankId', '<HAVALE_BANKA_ID>'),
    ('BlueParaBaseUrl',                 'https://api.alphaodeme.com/api'),  -- canlı sunucu
    ('BlueParaApiKey',                  '<BLUEPARA_API_KEY>'),
    ('BlueParaProviderKey',             '<BLUEPARA_PROVIDER_KEY>'),
    ('BlueParaMerchantKey',             ''),
    ('BlueParaCallbackUrl',             'https://<CALLBACKS_ALAN_ADI>/api/BluePara');

IF EXISTS (SELECT 1 FROM @Ayarlar WHERE [Value] LIKE '%<%')
BEGIN
    RAISERROR(N'Lütfen önce <...> ile işaretli alanları doldurun.', 16, 1);
    RETURN;
END

UPDATE s SET s.[Value] = a.[Value]
FROM SettingsV2 s
JOIN @Ayarlar a ON a.[Key] = s.[Key];

INSERT INTO SettingsV2 ([Key], [Value])
SELECT a.[Key], a.[Value]
FROM @Ayarlar a
WHERE NOT EXISTS (SELECT 1 FROM SettingsV2 s WHERE s.[Key] = a.[Key]);

-- Kontrol: 7 satır dönmeli
SELECT [Key], CASE WHEN [Key] LIKE '%Key' THEN '***' ELSE [Value] END AS [Value]
FROM SettingsV2
WHERE [Key] LIKE 'BluePara%';
