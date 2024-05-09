using System.Text.RegularExpressions;
using EBanking.Data;
using EBanking.Data.Entities;
using EBanking.Interfaces;

namespace EBanking.Services
{
    public class UserRegistrationService(EBankingDbContext dbContext) : IUserService
    {
        public User RegisterUser(string username, string password, string fullName, string email)
        {
            foreach (var user in dbContext.Users.All)
            {
                if (user.Username == username)
                {   
                    throw new ArgumentException("Потребителското име вече съществува.", nameof(username));
                }
            }
            if (string.IsNullOrEmpty((username)))    
                throw new ArgumentException("Username-ът е задължителен!", nameof(username));
            if (string.IsNullOrEmpty((password)))
                throw new ArgumentException("Паролата е задължителна!", nameof(password));
            if (string.IsNullOrEmpty((fullName)))
                throw new ArgumentException("Името е задължително!", nameof(fullName));
            if (string.IsNullOrEmpty((email)))
                throw new ArgumentException("Email-ът е задължително!", nameof(email));
            if (!IsUsernameValid(username))
                throw new ArgumentException("Некоректен формат на username-ът!", nameof(username));
            if (!IsPasswordValid(password))
                throw new ArgumentException("Некоректен формат на паролата!", nameof(password));
            if (!IsValidEmail(email))
                throw new ArgumentException("Невалидна електронна поща.", nameof(email));



            User newUser = new User
            {
                Username = username,
                Password = password,
                FullName = fullName,
                Email = email,
                DateRegistered = DateTime.Now
            };
            dbContext.Users.Insert(newUser);
            return newUser;

        }

        public User LoginUser(string username, string password)
        {
            var user = dbContext.Users.All.FirstOrDefault(u => u.Username == username && u.Password == password);
            if (user == null )
            {
                throw new ArgumentException("Невалидно потребителско име или парола.");
            }

            return user;
        }

       

        public IEnumerable<UserAccount> GetUserAccounts(int userId)
        {
            return dbContext.UserAccounts.All.Where(account => account.UserId == userId).ToList();
        }

        public IEnumerable<EBanking.Data.Entities.Transaction> GetAccountTransactions(int accountId)
        {
            return dbContext.Transactions.All.Where(transaction => transaction.UserAccountId == accountId).ToList();
        }

        private bool IsPasswordValid(string password)
        {
            return !string.IsNullOrWhiteSpace(password) && password.Length >= 8
                                                        && password.Any(char.IsLetter) && password.Any(char.IsDigit);
        }

        private static bool IsUsernameValid(string username)
        {
            return !string.IsNullOrWhiteSpace(username) && username.Length >= 4 && username.Length <= 16
                   && Regex.IsMatch(username, "^[a-zA-Z0-9]+$");
        }

        private static bool IsValidEmail(string email)
        {
            return !string.IsNullOrWhiteSpace(email) && Regex.IsMatch(email,
                @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
        }
    }
}
