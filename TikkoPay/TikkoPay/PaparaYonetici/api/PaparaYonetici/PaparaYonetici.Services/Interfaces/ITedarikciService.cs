using PaparaYonetici.Core.DTOs;
using PaparaYonetici.Core.DTOs.Tedarikciler;

namespace PaparaYonetici.Services.Interfaces
{
    public interface ITedarikciService
    {
        ResponseViewModel<TedarikcilerDto> GetById(Guid id);
        Task<ResponseViewModel<List<TedarikcilerListDto>>> GetAll();
        Task<ResponseViewModel<List<TedarikciBakiyeListDto>>> GetBakiyeList(Guid? tedarikciId);
        Task<ResponseViewModel<TedarikcilerDto>> Create(TedarikcilerCreateDto item);
        Task<ResponseViewModel<TedarikcilerDto>> Update(TedarikcilerUpdateDto itemDto);
        Task<ResponseViewModel<TedarikcilerDto>> Delete(Guid id);
    }
}
