using AutoMapper;
using TikkoPayYonetici.Core.DTOs.Affiliate;
using TikkoPayYonetici.Core.DTOs.Banka;
using TikkoPayYonetici.Core.DTOs.CekimTalepleri;
using TikkoPayYonetici.Core.DTOs.HavaleBankalari;
using TikkoPayYonetici.Core.DTOs.KasaHaraketleri;
using TikkoPayYonetici.Core.DTOs.KullaniciBlackList;
using TikkoPayYonetici.Core.DTOs.KullaniciIpAdresleri;
using TikkoPayYonetici.Core.DTOs.Kullanicilar;
using TikkoPayYonetici.Core.DTOs.KullaniciYetkileri;
using TikkoPayYonetici.Core.DTOs.Tedarikciler;
using TikkoPayYonetici.Core.DTOs.WebSiteKasaHaraketleri;
using TikkoPayYonetici.Core.DTOs.WebSiteleri;
using TikkoPayYonetici.Core.DTOs.YatirimTalepleri;
using TikkoPayYonetici.Core.Models;

namespace TikkoPayYonetici.Core.Profiles
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
        }
    }
}
