using System;

namespace EBanking.Data.Core
{
    public class DbTransaction : IDisposable
    {
        private readonly DbManager _dbManager;
        private bool _shouldRollback = true;

        internal EBankingDb CommittedState => _dbManager.CommittedState;
        internal EBankingDb TransactionState => _dbManager.TransactionState;

        internal bool StateModified
        {
            get => _dbManager.StateModified;
            set => _dbManager.StateModified = value;
        }

        internal DbTransaction(DbManager dbManager)
        {
            _dbManager = dbManager;
        }

        public void Commit()
        {
            _dbManager.CommitDbTransaction();
            _shouldRollback = false;
        }

        public void Dispose()
        {
            if (_shouldRollback)
                _dbManager.RollbackDbTransaction();
        }
    }
}
