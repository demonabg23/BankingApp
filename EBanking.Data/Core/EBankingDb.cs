using System.Collections.Generic;
using System.IO;
using System.Linq;
using EBanking.Data.Entities;
using Newtonsoft.Json;

namespace EBanking.Data.Core
{
    internal class EBankingDb
    {
        public int UserSeq { get; set; }
        public int UserAccountSeq { get; set; }
        public int TransactionSeq { get; set; }
        public List<User> Users { get; set; }
        public List<UserAccount> UserAccounts { get; set; }
        public List<Transaction> Transactions { get; set; }

        public EBankingDb()
        {
            Users = new List<User>();
            UserAccounts = new List<UserAccount>();
            Transactions = new List<Transaction>();
        }

        internal EBankingDb Clone()
        {
            return new EBankingDb
            {
                UserSeq = UserSeq,
                UserAccountSeq = UserAccountSeq,
                TransactionSeq = TransactionSeq,
                Users = Users.Select(x => x.Clone()).ToList(),
                UserAccounts = UserAccounts.Select(x => x.Clone()).ToList(),
                Transactions = Transactions.Select(x => x.Clone()).ToList()
            };
        }

        internal static EBankingDb Load(string dbPath)
        {
            using (var stream = new FileStream(
                dbPath, FileMode.OpenOrCreate, FileAccess.Read, FileShare.Read))
            using (var reader = new StreamReader(stream))
            {
                if (stream.Length == 0)
                    return new EBankingDb();

                var content = reader.ReadToEnd();
                
                return JsonConvert.DeserializeObject<EBankingDb>(content);
            }
        }

        internal static void Save(string dbPath, EBankingDb db)
        {
            using (var stream = new FileStream(
                dbPath, FileMode.Create, FileAccess.Write, FileShare.Read))
            using (var writer = new StreamWriter(stream))
            {
                var content = JsonConvert.SerializeObject(db);

                writer.Write(content);
            }
        }
    }
}
