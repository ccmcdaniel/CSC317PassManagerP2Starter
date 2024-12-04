/*
 * Program Author:  Reham Afzal 
 * ID: w10171356 
 * Assignment: Password Manager part 2
 * 
 * Description:
 * Implements the back end of a simple password manager app.
 */

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using CSC317PassManagerP2Starter.Modules.Models;
using CSC317PassManagerP2Starter.Modules.Views;

namespace CSC317PassManagerP2Starter.Modules.Controllers
{
    public class PasswordController
    {
        // Stores a list of sample passwords for the test user
        private List<PasswordModel> _passwords = new List<PasswordModel>();
        private int counter = 1;
        private readonly byte[] _key = Encoding.UTF8.GetBytes("SampleKey1234567"); // 16-byte key for AES
        private readonly byte[] _iv = Encoding.UTF8.GetBytes("SampleIV1234567");   // 16-byte IV for AES

        // Copies the passwords to the Row Binders with optional search criteria
        public void PopulatePasswordView(ObservableCollection<PasswordRow> source, string searchCriteria = "")
        {
            source.Clear();

            var filteredPasswords = string.IsNullOrEmpty(searchCriteria)
                ? _passwords
                : _passwords.Where(p =>
                    p.PlatformName.Contains(searchCriteria, StringComparison.OrdinalIgnoreCase) ||
                    p.PlatformUserName.Contains(searchCriteria, StringComparison.OrdinalIgnoreCase)).ToList();

            foreach (var password in filteredPasswords)
            {
                source.Add(new PasswordRow(password));
            }
        }

        // Adds a new password to the list
        public void AddPassword(string platform, string username, string password)
        {
            var encryptedPassword = EncryptPassword(password);
            _passwords.Add(new PasswordModel
            {
                ID = counter++,
                PlatformName = platform,
                PlatformUserName = username,
                PasswordText = Convert.ToBase64String(encryptedPassword) // Store as Base64
            });
        }

        // Retrieves a password by its ID
        public PasswordModel? GetPassword(int ID)
        {
            return _passwords.FirstOrDefault(p => p.ID == ID);
        }

        // Updates an existing password by its ID
        public bool UpdatePassword(PasswordModel changes)
        {
            var existingPassword = GetPassword(changes.ID);
            if (existingPassword != null)
            {
                existingPassword.PlatformName = changes.PlatformName;
                existingPassword.PlatformUserName = changes.PlatformUserName;
                existingPassword.PasswordText = changes.PasswordText;
                return true;
            }
            return false;
        }

        // Removes a password by its ID
        public bool RemovePassword(int ID)
        {
            var password = GetPassword(ID);
            if (password != null)
            {
                _passwords.Remove(password);
                return true;
            }
            return false;
        }

        // Generates a set of test passwords for demonstration purposes
        public void GenTestPasswords()
        {
            AddPassword("Facebook", "user1", "password1");
            AddPassword("Google", "user2", "password2");
            AddPassword("Twitter", "user3", "password3");
        }

        // Encrypts a password using AES encryption
        private byte[] EncryptPassword(string password)
        {
            using (var aes = Aes.Create())
            {
                aes.Key = _key;
                aes.IV = _iv;
                var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (var ms = new System.IO.MemoryStream())
                using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    using (var writer = new System.IO.StreamWriter(cs))
                    {
                        writer.Write(password);
                    }
                    return ms.ToArray();
                }
            }
        }

        // Decrypts an AES-encrypted password
        public string DecryptPassword(string encryptedPassword)
        {
            var encryptedBytes = Convert.FromBase64String(encryptedPassword);

            using (var aes = Aes.Create())
            {
                aes.Key = _key;
                aes.IV = _iv;
                var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (var ms = new System.IO.MemoryStream(encryptedBytes))
                using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                using (var reader = new System.IO.StreamReader(cs))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}
