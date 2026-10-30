using ParaYolYonetici.Core.DTOs;
using ParaYolYonetici.Core.DTOs.Tedarikciler;

namespace ParaYolYonetici.Services.Interfaces
{
    public interface ITedarikciService
    {
        ResponseViewModel<TedarikcilerDto> GetById(Guid id);
        Task<ResponseViewModel<List<TedarikcilerListDto>>> GetAll();
        Task<ResponseViewModel<List<TedarikcilerListDto>>> GetAllWithPaging(bool? bankaHavale, bool? otoPuan, bool? durum, int pageNumber, int recordsPerPage);
        Task<ResponseViewModel<List<TedarikcilerSelectList>>> GetSelectList();
        Task<ResponseViewModel<List<TedarikcilerSelectListWithBakiyeDto>>> GetSelectListWithBakiye();
        Task<ResponseViewModel<List<TedarikciBakiyeListDto>>> GetBakiyeList(Guid? tedarikciId, int? tedarikciDurum);
        Task<ResponseViewModel<TedarikcilerDto>> Create(TedarikcilerCreateDto item);
        Task<ResponseViewModel<TedarikcilerDto>> Update(TedarikcilerUpdateDto itemDto);
        Task<ResponseViewModel<TedarikcilerDto>> Delete(Guid id);
    }
}
