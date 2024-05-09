using EBanking.Data.Entities;

namespace EBanking.Interfaces
{
    public interface IUserService
    {
        User RegisterUser(string username, string password, string fullName, string email);

        User LoginUser(string username, string password);

        IEnumerable<UserAccount> GetUserAccounts(int userId);

        IEnumerable<Transaction> GetAccountTransactions(int accountId);

       
    }
}
