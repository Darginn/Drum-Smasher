using DSServer.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DSServer.Users
{
    public static class AccountManager
    {
        public static bool CreateAccount(string user, string pass)
        {
            using DB db = new DB();

            if (db.Accounts.Any(acc => acc.AccountName.Equals(user)))
                return false;

            byte[] salt = GenerateRandomSalt();
            byte[] passBytes = Encoding.UTF8.GetBytes(pass);
            byte[] hashedPass = HashPass(ref passBytes, salt);

            db.Accounts.Add(new Account(user, hashedPass, salt, 0, false, user, false, DateTime.MinValue));
            db.SaveChanges();
            return true;
        }

        public static Account TryLogin(string user, string pass)
        {
            using DB db = new DB();
            var acc = db.Accounts.FirstOrDefault(acc => acc.AccountName.Equals(user, StringComparison.CurrentCultureIgnoreCase));

            if (acc == null || acc.IsBanned)
                return null;

            byte[] passBytes = Encoding.UTF8.GetBytes(pass);
            byte[] hashedPass = HashPass(ref passBytes, acc.Salt);

            if (CompareByteArrays(hashedPass, acc.PasswordHash))
            {
                acc.LastLogin = DateTime.UtcNow;
                db.Accounts.Update(acc);
                db.SaveChanges();

                return acc;
            }

            return null;
        }

        /// <summary>
        /// Bans a user, this does not disconnect the user
        /// </summary>
        /// <param name="accId">Account Id</param>
        /// <returns>Account found</returns>
        public static bool Ban(long accId)
        {
            return SetBan(accId, true);
        }

        /// <summary>
        /// Unbans a user, this does not disconnect the user
        /// </summary>
        /// <param name="accId">Account Id</param>
        /// <returns>Account found</returns>
        public static bool Unban(long accId)
        {
            return SetBan(accId, false);
        }

        /// <summary>
        /// Silences a user, this does not update the see <see cref="ChatSystem.ChatUser"/>
        /// </summary>
        /// <param name="accId">Account Id</param>
        /// <returns>Account found</returns>
        public static bool Silence(long accId)
        {
            return SetSilence(accId, true);
        }

        /// <summary>
        /// Unsilences a user, this does not update the see <see cref="ChatSystem.ChatUser"/>
        /// </summary>
        /// <param name="accId">Account Id</param>
        /// <returns>Account found</returns>
        public static bool Unsilence(long accId)
        {
            return SetSilence(accId, false);
        }


        static byte[] GenerateRandomSalt()
        {
            byte[] salt = new byte[16];
            GenerateSalt();

            using DB db = new DB();

            while (db.Accounts.Any(acc => CompareByteArrays(acc.Salt, salt)))
                GenerateSalt();

            return salt;

            void GenerateSalt()
            {
                for (int i = 0; i < salt.Length; i++)
                    salt[i] = (byte)Program.Random.Next(0, 256);
            }
        }

        /// <summary>
        /// Hashes a password
        /// <para>
        /// Credits: https://stackoverflow.com/a/2138588 </para>
        /// </summary>
        static byte[] HashPass(ref byte[] pass, byte[] salt)
        {
            System.Security.Cryptography.HashAlgorithm algorithm = new System.Security.Cryptography.SHA256Managed();

            byte[] passHash = new byte[pass.Length + salt.Length];

            for (int i = 0; i < pass.Length; i++)
                passHash[i] = pass[i];

            for (int i = 0; i < salt.Length; i++)
                passHash[pass.Length + i] = salt[i];

            return algorithm.ComputeHash(passHash);
        }

        /// <summary>
        /// Credits: https://stackoverflow.com/a/2138588
        /// </summary>
        static bool CompareByteArrays(byte[] array1, byte[] array2)
        {
            if (array1.Length != array2.Length)
            {
                return false;
            }

            for (int i = 0; i < array1.Length; i++)
            {
                if (array1[i] != array2[i])
                {
                    return false;
                }
            }

            return true;
        }

        static bool SetBan(long accId, bool isBanned)
        {
            return UpdateAccount(accId, a => a.IsBanned = isBanned);
        }

        static bool SetSilence(long accId, bool isSilenced)
        {
            return UpdateAccount(accId, a => a.IsSilenced = isSilenced);
        }

        static bool UpdateAccount(long accId, Action<Account> updateAc)
        {
            using DB db = new DB();
            var acc = db.Accounts.First(acc => acc.Id == accId);

            if (acc == null)
                return false;

            updateAc(acc);

            db.Accounts.Update(acc);
            db.SaveChanges();

            return true;
        }
    }
}
