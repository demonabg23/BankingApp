namespace EBanking.Data.Core
{
    internal class DbManager
    {
        private readonly string _dbPath;
        private int _transactionCounter;
        private bool _shouldRollback;

        internal EBankingDb CommittedState { get; private set; }
        internal EBankingDb TransactionState { get; private set; }
        internal bool StateModified { get; set; }

        public DbManager(string dbPath)
        {
            _dbPath = dbPath;

            CommittedState = EBankingDb.Load(_dbPath);
        }

        internal DbTransaction StartDbTransaction()
        {
            if (_transactionCounter == 0)
            {
                _shouldRollback = false;

                TransactionState = CommittedState.Clone();
            }

            _transactionCounter++;

            return new DbTransaction(this);
        }

        internal void CommitDbTransaction()
        {
            if (_shouldRollback)
            {
                RollbackDbTransaction();

                return;
            }

            _transactionCounter--;

            if (_transactionCounter == 0)
            {
                if (StateModified)
                {
                    CommittedState = TransactionState.Clone();

                    EBankingDb.Save(_dbPath, CommittedState);
                }

                TransactionState = null;
                StateModified = false;
            }
        }
        
        internal void RollbackDbTransaction()
        {
            _transactionCounter--;
            _shouldRollback = true;
            
            if (_transactionCounter == 0)
            {
                TransactionState = null;
                StateModified = false;
            }
        }
    }
}
