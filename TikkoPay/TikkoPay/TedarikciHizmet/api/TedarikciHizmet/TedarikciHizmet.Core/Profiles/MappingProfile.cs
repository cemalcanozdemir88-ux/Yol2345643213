using AutoMapper;
using TedarikciHizmet.Core.DTOs.Banka;
using TedarikciHizmet.Core.DTOs.HavaleBankalari;
using TedarikciHizmet.Core.DTOs.KasaHaraketleri;
using TedarikciHizmet.Core.DTOs.Kullanicilar;
using TedarikciHizmet.Core.DTOs.KullaniciYetkileri;
using TedarikciHizmet.Core.DTOs.Tedarikciler;
using TedarikciHizmet.Core.DTOs.WebSiteKasaHaraketleri;
using TedarikciHizmet.Core.DTOs.WebSiteleri;
using TedarikciHizmet.Core.DTOs.YatirimTalepleri;
using TedarikciHizmet.Core.DTOs.YatirimTalepleriManual;
using TedarikciHizmet.Core.Models;

namespace TedarikciHizmet.Core.Profiles
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

            CreateMap<YatirimCekimTalepleriManual, YatirimTalepleriManualListDto>().ReverseMap();
            CreateMap<YatirimCekimTalepleriManual, YatirimTalepleriManualUpdateDto>().ReverseMap();

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
