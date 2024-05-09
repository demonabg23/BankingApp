using System;

namespace EBanking.Data.Entities
{
    public class UserAccount
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public Guid Key { get; set; }
        public string FriendlyName { get; set; }
        public decimal Balance { get; set; }

        internal UserAccount Clone()
        {
            return new UserAccount
            {
                Id = Id,
                UserId = UserId,
                Key = Key,
                FriendlyName = FriendlyName,
                Balance = Balance
            };
        }
    }
}
