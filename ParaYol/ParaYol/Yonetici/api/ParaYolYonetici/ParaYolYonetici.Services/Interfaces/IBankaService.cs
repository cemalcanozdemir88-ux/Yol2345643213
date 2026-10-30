using ParaYolYonetici.Core.DTOs;
using ParaYolYonetici.Core.DTOs.Banka;

namespace ParaYolYonetici.Services.Interfaces
{
    public interface IBankaService
    {
        Task<ResponseViewModel<BankaListDto>> GetById(Guid id);
        Task<ResponseViewModel<List<BankaListDto>>> GetAll();

        Task<ResponseViewModel<BankaListDto>> Create(BankaCreateDto item);

        Task<ResponseViewModel<BankaListDto>> Update(BankaUpdateDto item);

        Task<ResponseViewModel<BankaListDto>> Delete(Guid id);
    }
}
