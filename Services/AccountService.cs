using System;
using EBanking.Data;
using EBanking.Data.Entities;
using EBanking.Interfaces;
using Microsoft.Identity.Client;

namespace EBanking.Services
{
    public class AccountService(EBankingDbContext dbContext) : IAccountService
    {
        public UserAccount CreateAccount(int userId, string friendlyName)
        {
            Guid key = Guid.NewGuid();
            
            var newAccount = new UserAccount
            {
                UserId = userId,
                Key = key,
                FriendlyName = friendlyName,
                Balance = 0 
            };

            dbContext.UserAccounts.Insert(newAccount);

            return newAccount;
        }

        public void Transfer(int sendingAccountId, int receiverAccountId, decimal amount)
        {
            if (amount < 0)
            {
                throw new ArgumentException("Невалидна сума за превод", nameof(amount));
            }

            using var transaction = dbContext.StartDbTransaction();
            try
            {
                var sendingAccount = dbContext.UserAccounts.All.FirstOrDefault(a => a.Id == sendingAccountId);
                var receiverAccount = dbContext.UserAccounts.All.FirstOrDefault(a => a.Id == receiverAccountId);

                if (sendingAccount == null || receiverAccount == null)
                {
                    throw new ArgumentException("Невалидно Id на сметка");
                }

                if (sendingAccount.Balance < amount)
                {
                    throw new InvalidOperationException("Недостатъчно налични средства за превод");
                }

                var transactionDay = DateTime.UtcNow;

                var newTransactionFrom = new Transaction
                {
                    UserAccountId = sendingAccountId,
                    Type = TransactionType.Debit,
                    Amount = amount,
                    EventDate = transactionDay,
                    SystemComment = $"Извършен превод от сметка {sendingAccountId} към сметка {receiverAccountId}"
                };

                var newTransactionTo = new Transaction
                {
                    UserAccountId = receiverAccountId,
                    Type = TransactionType.Credit,
                    Amount = amount,
                    EventDate = transactionDay,
                    SystemComment = $"Извършен превод от сметка {sendingAccountId} към сметка {receiverAccountId}"
                };

                sendingAccount.Balance -= amount;
                receiverAccount.Balance += amount;
                dbContext.Transactions.Insert(newTransactionFrom);
                dbContext.Transactions.Insert(newTransactionTo);

                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Dispose();
                throw; 
            }
        }


        public bool Deposit(int accountId, decimal amount)
        {
            if (amount < 0)
                throw new ArgumentException("Невалидна стойност");
            

            var account = dbContext.UserAccounts.All.FirstOrDefault(a => a.Id == accountId);

            if (account == null)
             throw new ArgumentException("Невалидно Id");
            
            if (account.Balance < amount)
                return false;
            


            var transactionDay = DateTime.UtcNow;

            var newTransaction = new Transaction
            {
                UserAccountId = accountId,
                Type = TransactionType.Credit,
                Amount = amount,
                EventDate = transactionDay,
                SystemComment = $"Депозит по сметка {accountId}"
            };

            account.Balance += amount;
            dbContext.Transactions.Insert(newTransaction);

            return true;
        }

        public bool  Withdraw(int accountId, decimal amount)
        {
            if (amount < 0)
                throw new ArgumentException("Невалидна стойност");
            

            var account = dbContext.UserAccounts.All.FirstOrDefault(a => a.Id == accountId);

            if (account == null)
                throw new ArgumentException("Невалидно Id");
            

            decimal fee = Math.Max(amount * 0.001m, 0.10m);
            decimal totalAmount = amount + fee;

            if (account.Balance < totalAmount)
            {
                return false;
            }

            var transactionDay = DateTime.UtcNow;

            var withdrawalTransaction = new Transaction
            {
                UserAccountId = accountId,
                Type = TransactionType.Debit,
                Amount = amount,
                EventDate = transactionDay,
                SystemComment = $"Теглене от сметка {accountId}"
            };

            var feeTransaction = new Transaction
            {
                UserAccountId = accountId,
                Type = TransactionType.Debit,
                Amount = fee,
                EventDate = transactionDay,
                SystemComment = $"Такса за теглене от сметка {accountId}"
            };

            account.Balance -= totalAmount;

            dbContext.Transactions.Insert(withdrawalTransaction);
            dbContext.Transactions.Insert(feeTransaction);

            return true;
        }
    }
}