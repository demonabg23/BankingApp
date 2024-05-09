using System;

namespace EBanking.Data.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public DateTime DateRegistered { get; set; }

        internal User Clone()
        {
            return new User
            {
                Id = Id,
                Username = Username,
                Password = Password,
                FullName = FullName,
                Email = Email,
                DateRegistered = DateRegistered
            };
        }
    }
}
