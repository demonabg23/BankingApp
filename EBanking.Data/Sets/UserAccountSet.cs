using System;
using System.Collections.Generic;
using System.Linq;
using EBanking.Data.Core;
using EBanking.Data.Entities;
using EBanking.Data.Interfaces;

namespace EBanking.Data.Sets
{
    internal class UserAccountSet : IEntitySet<int, UserAccount>
    {
        private readonly DbManager _dbManager;

        public IEnumerable<UserAccount> All
        {
            get
            {
                using (var transaction = _dbManager.StartDbTransaction())
                {
                    var result = transaction.TransactionState.UserAccounts;

                    transaction.Commit();

                    return result.Select(x => x.Clone());
                }
            }
        }

        internal UserAccountSet(DbManager dbManager)
        {
            _dbManager = dbManager ?? throw new ArgumentNullException(nameof(dbManager));
        }

        public void Insert(UserAccount entity)
        {
            using (var transaction = _dbManager.StartDbTransaction())
            {
                transaction.StateModified = true;

                var currentId = ++transaction.TransactionState.UserAccountSeq;
                
                AssertData(transaction, entity);

                entity.Id = currentId;

                var clone = entity.Clone();

                transaction.TransactionState.UserAccounts.Add(clone);
                transaction.Commit();
            }
        }

        public void Update(UserAccount entity)
        {
            using (var transaction = _dbManager.StartDbTransaction())
            {
                transaction.StateModified = true;

                var oldEntity = transaction.TransactionState.UserAccounts
                    .SingleOrDefault(x => x.Id == entity.Id);

                if (oldEntity == null)
                    throw new Exception("UserAccount not found.");

                AssertData(transaction, entity);

                oldEntity.UserId = entity.UserId;
                oldEntity.Key = entity.Key;
                oldEntity.FriendlyName = entity.FriendlyName;
                oldEntity.Balance = entity.Balance;

                transaction.Commit();
            }
        }
        
        public void Delete(int id)
        {
            using (var transaction = _dbManager.StartDbTransaction())
            {
                transaction.StateModified = true;

                var oldEntity = transaction.TransactionState.UserAccounts
                    .SingleOrDefault(x => x.Id == id);

                if (oldEntity == null)
                    throw new Exception("UserAccount not found.");

                var userHasRelations = transaction.TransactionState.Transactions
                    .Any(x => x.UserAccountId == id);

                if (userHasRelations)
                    throw new Exception("There are Transactions related to this UserAccount.");

                transaction.TransactionState.UserAccounts.Remove(oldEntity);
                transaction.Commit();
            }
        }

        private void AssertData(DbTransaction transaction, UserAccount entity)
        {
            if (entity.FriendlyName == null)
                throw new Exception("FriendlyName is required.");

            var userMissing = transaction.TransactionState.Users
                .All(x => x.Id != entity.UserId);

            if (userMissing)
                throw new Exception($"User with Id={entity.UserId} not found.");
        }
    }
}
