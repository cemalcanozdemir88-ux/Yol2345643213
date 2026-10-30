using TedarikciHizmet.Core.DTOs;
using TedarikciHizmet.Core.DTOs.Tedarikciler;

namespace TedarikciHizmet.Services.Interfaces
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
