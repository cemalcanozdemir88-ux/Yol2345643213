using AutoMapper;
using PayFixYonetici.Core.DTOs.Affiliate;
using PayFixYonetici.Core.DTOs.CekimTalepleri;
using PayFixYonetici.Core.DTOs.KasaHaraketleri;
using PayFixYonetici.Core.DTOs.Kullanicilar;
using PayFixYonetici.Core.DTOs.KullaniciYetkileri;
using PayFixYonetici.Core.DTOs.WebSiteKasaHaraketleri;
using PayFixYonetici.Core.DTOs.WebSiteleri;
using PayFixYonetici.Core.DTOs.YatirimTalepleri;
using PayFixYonetici.Core.Models;

namespace PayFixYonetici.Core.Profiles
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
