/*
 * Program Author:  Reham Afzal 
 * ID: w10171356 
 * Assignment: Password Manager part 2
 * 
 * Description:
 * This program implements the back end of a simple password manager app.
 */

using CSC317PassManagerP2Starter.Modules.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace CSC317PassManagerP2Starter.Modules.Controllers
{
    public enum AuthenticationError { NONE, INVALIDUSERNAME, INVALIDPASSWORD }

    public class LoginController
    {
        private User _user = new User(); // Simulated user database
        private bool _loggedIn = false;  // Tracks login status

        public LoginController()
        {
            // Initialize a test user with default credentials
            _user = new User
            {
                ID = 1,
                FirstName = "John",
                LastName = "Doe",
                UserName = "test",
                PasswordHash = HashPassword("ab1234"),
                Key = GenerateKey(),
                IV = GenerateIV()
            };
        }

        public User? CurrentUser
        {
            get
            {
                // Returns a copy of the user data if logged in, otherwise null.
                return _loggedIn ? new User
                {
                    ID = _user.ID,
                    FirstName = _user.FirstName,
                    LastName = _user.LastName,
                    UserName = _user.UserName,
                    Key = _user.Key,
                    IV = _user.IV
                } : null;
            }
        }

        public AuthenticationError Authenticate(string username, string password)
        {
            // Validate the username
            if (username != _user.UserName)
                return AuthenticationError.INVALIDUSERNAME;

            // Validate the password hash
            if (!CompareHashes(HashPassword(password), _user.PasswordHash))
                return AuthenticationError.INVALIDPASSWORD;

            // Successful login
            _loggedIn = true;
            return AuthenticationError.NONE;
        }

        private byte[] HashPassword(string password)
        {
            // Hash the password using SHA-256 for better security
            using (var sha256 = SHA256.Create())
            {
                return sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        private bool CompareHashes(byte[] hash1, byte[] hash2)
        {
            // Compare two byte arrays securely
            return StructuralComparisons.StructuralEqualityComparer.Equals(hash1, hash2);
        }

        private byte[] GenerateKey()
        {
            // Generate a dummy 16-byte AES key (placeholder for actual encryption)
            return Encoding.UTF8.GetBytes("SampleKey1234567"); // 16 characters = 128 bits
        }

        private byte[] GenerateIV()
        {
            // Generate a dummy 16-byte AES IV (placeholder for actual encryption)
            return Encoding.UTF8.GetBytes("SampleIV1234567"); // 16 characters = 128 bits
        }
    }
}
