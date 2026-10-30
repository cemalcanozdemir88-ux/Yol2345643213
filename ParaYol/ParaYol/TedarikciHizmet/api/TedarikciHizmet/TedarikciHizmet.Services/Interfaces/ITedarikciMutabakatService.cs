using TedarikciHizmet.Core.DTOs;
using TedarikciHizmet.Core.DTOs.TedarikciMutabakatlari;

namespace TedarikciHizmet.Services.Interfaces
{
    public interface ITedarikciMutabakatService
    {
        Task<ResponseViewModel<List<TedarikciMutabakatlariListDto>>> GetAllDtoAsync(Guid tedarikciId, DateOnly mutabakatTarihi);
    }
}
