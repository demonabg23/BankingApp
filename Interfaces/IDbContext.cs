using EBanking.Data.Core;

namespace EBanking.Data.Interfaces
{
    public interface IDbContext
    {
        DbTransaction StartDbTransaction();
    }
}
