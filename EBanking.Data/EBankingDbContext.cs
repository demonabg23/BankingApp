using System;
using EBanking.Data.Core;
using EBanking.Data.Entities;
using EBanking.Data.Interfaces;
using EBanking.Data.Sets;

namespace EBanking.Data
{
    public class EBankingDbContext : IEbankingDbContext
    {
        private readonly DbManager _dbManager;

        public IEntitySet<int, User> Users { get; }
        public IEntitySet<int, UserAccount> UserAccounts { get; }
        public IEntitySet<int, Transaction> Transactions { get; }

        public EBankingDbContext(string dbPath)
        {
            if (string.IsNullOrWhiteSpace(dbPath))
                throw new ArgumentNullException(nameof(dbPath));

            _dbManager = new DbManager(dbPath);

            Users = new UserSet(_dbManager);
            UserAccounts = new UserAccountSet(_dbManager);
            Transactions = new TransactionSet(_dbManager);
        }
        
        public DbTransaction StartDbTransaction()
        {
            return _dbManager.StartDbTransaction();
        }
    }
}
