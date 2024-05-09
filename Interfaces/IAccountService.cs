using EBanking.Data.Entities;

namespace EBanking.Interfaces
{
    public interface IAccountService
    {
        UserAccount CreateAccount(int userId, string friendlyName);

        void Transfer(int sendingAccountId, int receiverAccountId, decimal amount);

        bool Deposit(int accountId, decimal amount);

        bool Withdraw(int accountId, decimal amount);
        
    }
}
