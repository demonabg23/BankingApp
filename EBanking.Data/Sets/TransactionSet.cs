using System;
using System.Collections.Generic;
using System.Linq;
using EBanking.Data.Core;
using EBanking.Data.Entities;
using EBanking.Data.Interfaces;

namespace EBanking.Data.Sets
{
    internal class TransactionSet : IEntitySet<int, Transaction>
    {
        private readonly DbManager _dbManager;

        public IEnumerable<Transaction> All
        {
            get
            {
                using (var transaction = _dbManager.StartDbTransaction())
                {
                    var result = transaction.TransactionState.Transactions;

                    transaction.Commit();

                    return result.Select(x => x.Clone());
                }
            }
        }

        internal TransactionSet(DbManager dbManager)
        {
            _dbManager = dbManager ?? throw new ArgumentNullException(nameof(dbManager));
        }

        public void Insert(Transaction entity)
        {
            using (var transaction = _dbManager.StartDbTransaction())
            {
                transaction.StateModified = true;

                var currentId = ++transaction.TransactionState.TransactionSeq;
                
                AssertData(transaction, entity);

                entity.Id = currentId;

                var clone = entity.Clone();

                transaction.TransactionState.Transactions.Add(clone);
                transaction.Commit();
            }
        }

        public void Update(Transaction entity)
        {
            using (var transaction = _dbManager.StartDbTransaction())
            {
                transaction.StateModified = true;

                var oldEntity = transaction.TransactionState.Transactions
                    .SingleOrDefault(x => x.Id == entity.Id);

                if (oldEntity == null)
                    throw new Exception("Transaction not found.");

                AssertData(transaction, entity);

                oldEntity.UserAccountId = entity.UserAccountId;
                oldEntity.Key = entity.Key;
                oldEntity.Type = entity.Type;
                oldEntity.EventDate = entity.EventDate;
                oldEntity.Amount = entity.Amount;
                oldEntity.SystemComment = entity.SystemComment;

                transaction.Commit();
            }
        }
        
        public void Delete(int id)
        {
            using (var transaction = _dbManager.StartDbTransaction())
            {
                transaction.StateModified = true;

                var oldEntity = transaction.TransactionState.Transactions
                    .SingleOrDefault(x => x.Id == id);

                if (oldEntity == null)
                    throw new Exception("Transaction not found.");

                transaction.TransactionState.Transactions.Remove(oldEntity);
                transaction.Commit();
            }
        }

        private void AssertData(DbTransaction transaction, Transaction entity)
        {
            var userAccountMissing = transaction.TransactionState.UserAccounts
                .All(x => x.Id != entity.UserAccountId);

            if (userAccountMissing)
                throw new Exception($"UserAccount with Id={entity.UserAccountId} not found.");
        }
    }
}
