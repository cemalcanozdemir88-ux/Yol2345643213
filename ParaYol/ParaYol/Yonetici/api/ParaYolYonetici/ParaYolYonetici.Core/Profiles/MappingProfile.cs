using AutoMapper;
using ParaYolYonetici.Core.DTOs.Affiliate;
using ParaYolYonetici.Core.DTOs.Banka;
using ParaYolYonetici.Core.DTOs.CekimTalepleri;
using ParaYolYonetici.Core.DTOs.HavaleBankalari;
using ParaYolYonetici.Core.DTOs.KasaHaraketleri;
using ParaYolYonetici.Core.DTOs.KullaniciBlackList;
using ParaYolYonetici.Core.DTOs.KullaniciIpAdresleri;
using ParaYolYonetici.Core.DTOs.Kullanicilar;
using ParaYolYonetici.Core.DTOs.KullaniciYetkileri;
using ParaYolYonetici.Core.DTOs.PaymentMethods;
using ParaYolYonetici.Core.DTOs.Tedarikciler;
using ParaYolYonetici.Core.DTOs.WebSiteKasaHaraketleri;
using ParaYolYonetici.Core.DTOs.WebSiteleri;
using ParaYolYonetici.Core.DTOs.YatirimTalepleri;
using ParaYolYonetici.Core.DTOs.YatirimTalepleriManual;
using ParaYolYonetici.Core.Models;

namespace ParaYolYonetici.Core.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Kullanicilar, KullanicilarListDto>().ReverseMap();
            CreateMap<Kullanicilar, KullanicilarCreateDto>().ReverseMap();
            CreateMap<Kullanicilar, KullanicilarUpdateDto>().ReverseMap();

            CreateMap<KullaniciYetkileri, KullaniciYetkileriListDto>().ReverseMap();
            CreateMap<KullaniciYetkileri, KullaniciYetkileriCreateDto>().ReverseMap();
            CreateMap<KullaniciYetkileri, KullaniciYetkileriUpdateDto>().ReverseMap();

            CreateMap<Tedarikciler, TedarikcilerDto>().ReverseMap();
            CreateMap<Tedarikciler, TedarikcilerListDto>().ReverseMap();
            CreateMap<Tedarikciler, TedarikcilerCreateDto>().ReverseMap();
            CreateMap<Tedarikciler, TedarikcilerUpdateDto>().ReverseMap();

            CreateMap<Bankalar, BankaListDto>().ReverseMap();
            CreateMap<Bankalar, BankaCreateDto>().ReverseMap();
            CreateMap<Bankalar, BankaUpdateDto>().ReverseMap();

            CreateMap<HavaleBankalari, HavaleBankalariListDto>().ReverseMap();
            CreateMap<HavaleBankalari, HavaleBankalariCreateDto>().ReverseMap();
            CreateMap<HavaleBankalari, HavaleBankalariUpdateDto>().ReverseMap();

            CreateMap<YatirimCekimTalepleri, YatirimTalepleriListDto>().ReverseMap();
            CreateMap<YatirimCekimTalepleri, YatirimTalepleriCreateDto>().ReverseMap();
            CreateMap<YatirimCekimTalepleri, YatirimTalepleriUpdateDto>().ReverseMap();

            CreateMap<YatirimCekimTalepleri, CekimTalepleriListDto>().ReverseMap();
            CreateMap<YatirimCekimTalepleri, CekimTalepleriCreateDto>().ReverseMap();
            CreateMap<YatirimCekimTalepleri, CekimTalepleriUpdateDto>().ReverseMap();

            CreateMap<YatirimCekimTalepleriManual, YatirimTalepleriManualListDto>().ReverseMap();

            CreateMap<KasaHaraketleri, KasaHaraketleriListDto>().ReverseMap();
            CreateMap<KasaHaraketleri, KasaHaraketleriCreateDto>().ReverseMap();
            CreateMap<KasaHaraketleri, KasaHaraketleriUpdateDto>().ReverseMap();

            CreateMap<WebSiteKasaHaraketleri, WebSiteKasaHaraketleriListDto>().ReverseMap();
            CreateMap<WebSiteKasaHaraketleri, WebSiteKasaHaraketleriCreateDto>().ReverseMap();
            CreateMap<WebSiteKasaHaraketleri, WebSiteKasaHaraketleriUpdateDto>().ReverseMap();

            CreateMap<WebSiteleri, WebSiteleriDto>().ReverseMap();
            CreateMap<WebSiteleri, WebSiteleriListDto>().ReverseMap();
            CreateMap<WebSiteleri, WebSiteleriCreateDto>().ReverseMap();
            CreateMap<WebSiteleri, WebSiteleriUpdateDto>().ReverseMap();

            CreateMap<Affiliates, AffiliateDto>().ReverseMap();
            CreateMap<Affiliates, AffiliateCreateDto>().ReverseMap();
            CreateMap<Affiliates, AffiliateUpdateDto>().ReverseMap();

            CreateMap<KullaniciBlackLists, KullaniciBlackListDto>().ReverseMap();
            CreateMap<KullaniciBlackLists, KullaniciBlackListsCreateDto>().ReverseMap();

            CreateMap<KullaniciIpAdresleri, KullaniciIpAdresleriListDto>().ReverseMap();
            CreateMap<KullaniciIpAdresleri, KullaniciIpAdresleriCreateDto>().ReverseMap();
            CreateMap<KullaniciIpAdresleri, KullaniciIpAdresleriUpdateDto>().ReverseMap();

            CreateMap<PaymentMethods, PaymentMethodsDto>().ReverseMap();
        }
    }
}
