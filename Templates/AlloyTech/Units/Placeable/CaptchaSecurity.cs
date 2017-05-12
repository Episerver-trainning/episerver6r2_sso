#region Copyright
// Copyright © EPiServer AB.  All rights reserved.
// 
// This code is released by EPiServer AB under the Source Code File - Specific License Conditions, published August 20, 2007. 
// See http://www.episerver.com/Specific_License_Conditions for details.
#endregion
using System;
using System.Security.Cryptography;
using System.Text;

namespace EPiServer.Templates.AlloyTech.Units.Placeable
{
    /// <summary>
    /// Helper class for encryption of captcha text when transfered to via the client browser
    /// </summary>
    public static class CaptchaSecurity
    {
        private static byte[] _siteCaptchaTripleDESCryptoKey;
        private static object _lockObject = new object();

        /// <summary>
        /// Encrypts a string with 3DES encryption using a key stored in SiteConfig
        /// </summary>
        /// <param name="text">The text to encrypt</param>
        /// <returns>The ebncrypted string base64 encoded</returns>
        public static string Encrypt(string text)
        {
            byte[] data = Encoding.UTF8.GetBytes(text);
            return Convert.ToBase64String(TripleDESEncrypt(data, SiteCaptchaTripleDESCryptoKey));
        }

        /// <summary>
        /// Decrypts a 3DES encrypted string returned by <see cref="Encrypt"/>.
        /// </summary>
        /// <param name="text">The string to decrypt</param>
        /// <returns>The string in cleartext.</returns>
        public static string Decrypt(string text)
        {
            byte[] data = Convert.FromBase64String(text);
            return Encoding.UTF8.GetString(TripleDESDecrypt(data, SiteCaptchaTripleDESCryptoKey));
        }

        /// <summary>
        /// Gets a 24 bytes array for encrypting data with 3DES
        /// </summary>
        private static byte[] SiteCaptchaTripleDESCryptoKey
        {
            get
            {
                lock (_lockObject)
                {
                    if (_siteCaptchaTripleDESCryptoKey == null)
                    {
                        _siteCaptchaTripleDESCryptoKey = GetCryptoKey("SiteCaptchaCryptoKey", 24);
                    }
                    return _siteCaptchaTripleDESCryptoKey;
                }
            }
        }

        /// <summary>
        /// Encrypts a byte array with 3DES using the provided key.
        /// </summary>
        /// <param name="data">The data to encrypt.</param>
        /// <param name="key">The encryption as a byte array with length 24.</param>
        /// <returns>An encrypted byte array</returns>
        private static byte[] TripleDESEncrypt(byte[] data, byte[] key)
        {
            byte[] iv = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 };
            ICryptoTransform cTranform = new TripleDESCryptoServiceProvider().CreateEncryptor(key, iv);
            return cTranform.TransformFinalBlock(data, 0, data.Length);
        }

        /// <summary>
        /// Decrypts a byte array encrypted with 3DES using the provided key.
        /// </summary>
        /// <param name="data">The data to decrypt.</param>
        /// <param name="key">The encryption as a byte array with length 24.</param>
        /// <returns>An decrypted byte array</returns>
        private static byte[] TripleDESDecrypt(byte[] data, byte[] key)
        {
            byte[] iv = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 };
            ICryptoTransform cTranform = new TripleDESCryptoServiceProvider().CreateDecryptor(key, iv);
            return cTranform.TransformFinalBlock(data, 0, data.Length);
        }

        /// <summary>
        /// Returns a byte array of specified length suitable for encryption.
        /// The byte array is fetched from site settings for the given keyName.
        /// If no key exists for the keyName a new key with the specified length
        /// is created and stored in site settings.
        /// </summary>
        /// <param name="keyName">Name of a stored key.</param>
        /// <param name="keyLength">Length of the key. Used when creating a new key.</param>
        /// <returns>A random byte array of specified length.</returns>
        private static byte[] GetCryptoKey(string keyName, int keyLength)
        {
            byte[] cryptoKey;
            string siteShortName = EPiServer.Configuration.Settings.Instance.Parent.SiteId;

            EPiServer.DataAccess.SiteConfigDB siteConfigDB = new EPiServer.DataAccess.SiteConfigDB();
            string cryptoString = siteConfigDB.GetValue(siteShortName, keyName);

            if (string.IsNullOrEmpty(cryptoString))
            {
                cryptoKey = GenerateRandomByteSequence(keyLength);
                siteConfigDB.SetValue(siteShortName, keyName, Convert.ToBase64String(cryptoKey));
            }
            else
            {
                cryptoKey = Convert.FromBase64String(cryptoString);
            }
            return cryptoKey;
        }

        /// <summary>
        /// Returns a sequence of random bytes suitable for encryption.
        /// </summary>
        /// <param name="length">Requested length for the byte array returned</param>
        /// <returns>A byte array with random data.</returns>
        public static byte[] GenerateRandomByteSequence(int length)
        {
            byte[] randomData = new byte[length];
            RNGCryptoServiceProvider rngProvider = new RNGCryptoServiceProvider();
            rngProvider.GetBytes(randomData);
            return randomData;
        }
    }
}
