using TedarikciHizmet.Core.DTOs;
using TedarikciHizmet.Core.DTOs.TedarikciMutabakatlari;
using TedarikciHizmet.Core.Interfaces;
using TedarikciHizmet.Services.Interfaces;

namespace TedarikciHizmet.Services.Concretes
{
    public class TedarikciMutabakatService : ITedarikciMutabakatService
    {
        private readonly ITedarikciMutabakatRepository _tedarikciMutabakatRepository;

        public TedarikciMutabakatService(ITedarikciMutabakatRepository tedarikciMutabakatRepository)
        {
            _tedarikciMutabakatRepository = tedarikciMutabakatRepository;
        }

        public async Task<ResponseViewModel<List<TedarikciMutabakatlariListDto>>> GetAllDtoAsync(Guid tedarikciId, DateOnly mutabakatTarihi)
        {
            var mutabakatList = await _tedarikciMutabakatRepository.GetAllDtoAsync(tedarikciId, mutabakatTarihi);

            return new ResponseViewModel<List<TedarikciMutabakatlariListDto>>
            {
                Success = true,
                Result = mutabakatList.ToList()
            };
        }
    }
}
