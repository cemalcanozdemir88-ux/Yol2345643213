using AutoMapper;
using PayCoYonetici.Core.DTOs.Affiliate;
using PayCoYonetici.Core.DTOs.CekimTalepleri;
using PayCoYonetici.Core.DTOs.KasaHaraketleri;
using PayCoYonetici.Core.DTOs.Kullanicilar;
using PayCoYonetici.Core.DTOs.KullaniciYetkileri;
using PayCoYonetici.Core.DTOs.WebSiteKasaHaraketleri;
using PayCoYonetici.Core.DTOs.WebSiteleri;
using PayCoYonetici.Core.DTOs.YatirimTalepleri;
using PayCoYonetici.Core.Models;

namespace PayCoYonetici.Core.Profiles
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

            CreateMap<YatirimTalepleri, YatirimTalepleriListDto>().ReverseMap();
            CreateMap<CekimTalepleri, CekimTalepleriListDto>().ReverseMap();
        }
    }
}
