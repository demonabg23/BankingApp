using System;
using System.Collections.Generic;
using System.Linq;
using EBanking.Data.Core;
using EBanking.Data.Entities;
using EBanking.Data.Interfaces;

namespace EBanking.Data.Sets
{
    internal class UserSet : IEntitySet<int, User>
    {
        private readonly DbManager _dbManager;

        public IEnumerable<User> All
        {
            get
            {
                using (var transaction = _dbManager.StartDbTransaction())
                {
                    var result = transaction.TransactionState.Users;

                    transaction.Commit();

                    return result.Select(x => x.Clone());
                }
            }
        }

        internal UserSet(DbManager dbManager)
        {
            _dbManager = dbManager ?? throw new ArgumentNullException(nameof(dbManager));
        }

        public void Insert(User entity)
        {
            using (var transaction = _dbManager.StartDbTransaction())
            {
                transaction.StateModified = true;

                var currentId = ++transaction.TransactionState.UserSeq;
                
                AssertData(transaction, entity);

                entity.Id = currentId;
                
                var clone = entity.Clone();

                transaction.TransactionState.Users.Add(clone);
                transaction.Commit();
            }
        }

        public void Update(User entity)
        {
            using (var transaction = _dbManager.StartDbTransaction())
            {
                transaction.StateModified = true;

                var oldEntity = transaction.TransactionState.Users
                    .SingleOrDefault(x => x.Id == entity.Id);

                if (oldEntity == null)
                    throw new Exception("User not found.");

                AssertData(transaction, entity);

                oldEntity.Username = entity.Username;
                oldEntity.Password = entity.Password;
                oldEntity.FullName = entity.FullName;
                oldEntity.Email = entity.Email;
                oldEntity.DateRegistered = entity.DateRegistered;

                transaction.Commit();
            }
        }
        
        public void Delete(int id)
        {
            using (var transaction = _dbManager.StartDbTransaction())
            {
                transaction.StateModified = true;

                var oldEntity = transaction.TransactionState.Users
                    .SingleOrDefault(x => x.Id == id);

                if (oldEntity == null)
                    throw new Exception("User not found.");

                var userHasRelations = transaction.TransactionState.UserAccounts
                    .Any(x => x.UserId == id);

                if (userHasRelations)
                    throw new Exception("There are UserAccounts related to this User.");

                transaction.TransactionState.Users.Remove(oldEntity);
                transaction.Commit();
            }
        }

        private void AssertData(DbTransaction transaction, User entity)
        {
            if (entity.Username == null)
                throw new Exception("Username is required.");
                
            if (entity.Password == null)
                throw new Exception("Password is required.");

            if (entity.FullName == null)
                throw new Exception("FullName is required.");

            if (entity.Email == null)
                throw new Exception("Email is required.");
        }
    }
}
