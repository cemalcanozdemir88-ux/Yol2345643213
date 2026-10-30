using AutoMapper;
using PaparaYonetici.Core.DTOs.Affiliate;
using PaparaYonetici.Core.DTOs.Banka;
using PaparaYonetici.Core.DTOs.HavaleBankalari;
using PaparaYonetici.Core.DTOs.KasaHaraketleri;
using PaparaYonetici.Core.DTOs.Kullanicilar;
using PaparaYonetici.Core.DTOs.KullaniciYetkileri;
using PaparaYonetici.Core.DTOs.Tedarikciler;
using PaparaYonetici.Core.DTOs.WebSiteKasaHaraketleri;
using PaparaYonetici.Core.DTOs.WebSiteleri;
using PaparaYonetici.Core.DTOs.YatirimTalepleri;
using PaparaYonetici.Core.Models;

namespace PaparaYonetici.Core.Profiles
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
        }
    }
}
