namespace Transactions.Infrastructure.Repositories.Interfaces
{
    public interface ITedarikcilerRepository
    {
        Task<decimal> GetYatirimKomisyonu(Guid tedarikciId, decimal tutar);
    }
}
