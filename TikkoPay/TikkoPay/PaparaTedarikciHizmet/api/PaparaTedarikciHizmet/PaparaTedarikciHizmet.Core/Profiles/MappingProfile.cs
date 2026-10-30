using AutoMapper;
using PaparaTedarikciHizmet.Core.DTOs.Banka;
using PaparaTedarikciHizmet.Core.DTOs.HavaleBankalari;
using PaparaTedarikciHizmet.Core.DTOs.KasaHaraketleri;
using PaparaTedarikciHizmet.Core.DTOs.Kullanicilar;
using PaparaTedarikciHizmet.Core.DTOs.KullaniciYetkileri;
using PaparaTedarikciHizmet.Core.DTOs.Tedarikciler;
using PaparaTedarikciHizmet.Core.DTOs.WebSiteKasaHaraketleri;
using PaparaTedarikciHizmet.Core.DTOs.WebSiteleri;
using PaparaTedarikciHizmet.Core.DTOs.YatirimTalepleri;
using PaparaTedarikciHizmet.Core.Models;

namespace PaparaTedarikciHizmet.Core.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Kullanicilar, KullanicilarListDto>().ReverseMap();
            CreateMap<Kullanicilar, KullanicilarCreateDto>().ReverseMap();
            CreateMap<Kullanicilar, KullanicilarUpdateDto>().ReverseMap();
            CreateMap<KullanicilarUpdateDto, KullanicilarListDto>().ReverseMap();

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
        }
    }
}
