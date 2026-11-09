using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace TikkoPayYonetici.Infrastructure.Repositories
{
    internal static class MutabakatHelper
    {
        /// <summary>
        /// SQL Server benzersizlik (unique index / unique key) ihlali mi?
        /// 2601: unique index ihlali, 2627: unique constraint ihlali.
        /// </summary>
        public static bool IsUniqueViolation(DbUpdateException ex)
        {
            return ex.InnerException is SqlException sqlEx && (sqlEx.Number == 2601 || sqlEx.Number == 2627);
        }
    }
}
