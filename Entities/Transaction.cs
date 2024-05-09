using System;

namespace EBanking.Data.Entities
{
    public class Transaction
    {
        public int Id { get; set; }
        public int UserAccountId { get; set; }
        public Guid Key { get; set; }
        public TransactionType Type { get; set; }
        public decimal Amount { get; set; }
        public DateTime EventDate { get; set; }
        public string SystemComment { get; set; }

        internal Transaction Clone()
        {
            return new Transaction
            {
                Id = Id,
                UserAccountId = UserAccountId,
                Key = Key,
                Type = Type,
                Amount = Amount,
                EventDate = EventDate,
                SystemComment = SystemComment
            };
        }
    }

    public enum TransactionType
    {
        Debit = 1,
        Credit
    }
}
