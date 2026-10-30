using TikkoPayYonetici.Core.DTOs;
using TikkoPayYonetici.Core.DTOs.TedarikciMutabakatlari;

namespace TikkoPayYonetici.Services.Interfaces
{
    public interface ITedarikciMutabakatService
    {
        Task<ResponseViewModel<List<TedarikciMutabakatlariListDto>>> GetAllDtoAsync(Guid? tedarikciId, DateOnly mutabakatTarihi);
        Task<ResponseViewModel<TedarikciMutabakatlariListDto>> Calculate(Guid tedarikciId, decimal devir, DateOnly mutabakatTarihi);
    }
}
