using ParaYolYonetici.Core.DTOs;
using ParaYolYonetici.Core.DTOs.TedarikciMutabakatlari;

namespace ParaYolYonetici.Services.Interfaces
{
    public interface ITedarikciMutabakatService
    {
        Task<ResponseViewModel<List<TedarikciMutabakatlariListDto>>> GetAllDtoAsync(Guid? tedarikciId, DateOnly mutabakatTarihi);
        Task<ResponseViewModel<TedarikciMutabakatlariListDto>> Calculate(Guid tedarikciId, decimal devir, DateOnly mutabakatTarihi);
    }
}
