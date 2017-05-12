#region Copyright
// Copyright © EPiServer AB.  All rights reserved.
// 
// This code is released by EPiServer AB under the Source Code File - Specific License Conditions, published August 20, 2007. 
// See http://www.episerver.com/Specific_License_Conditions for details.
#endregion
using System;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using EPiServer.Configuration;
using EPiServer.Core;
using EPiServer.DataAccess;

namespace EPiServer.Templates.Advanced.Workroom.Core
{
    /// <summary>
    /// Class for encoding and decoding invitation ticket
    /// </summary>
    public class InvitationTicket
    {
        /// <summary>
        /// Name of crypto key in DB
        /// </summary>
        private const string cryptoKeyName = "WorkroomInvitationCryptoKey";

        /// <summary>
        /// Separator for invitation ticket
        /// </summary>
        private const char separator = ';';

        /// <summary>
        /// Bytes for IV key
        /// </summary>
        private static readonly byte[] iv = new byte[] {0, 0, 0, 0, 0, 0, 0, 0};

        /// <summary>
        /// Lock object to get key form DB
        /// </summary>
        private static readonly object lockObject = new object();

        /// <summary>
        /// Crypto key used for crypting
        /// </summary>
        private static byte[] cryptoKey;

        /// <summary>
        /// Creates invitation
        /// </summary>
        /// <param name="workroomStartPage">Id of workroom</param>
        /// <param name="membershipLevel">User access level</param>
        /// <param name="email">Email of user</param>
        public InvitationTicket(PageReference workroomStartPage, MembershipLevels membershipLevel, string email)
        {
            WorkroomStartPage = workroomStartPage;
            MembershipLevel = membershipLevel;
            Email = email;
        }

        /// <summary>
        /// Gets a 24 bytes array for encrypting data with 3DES
        /// </summary>
        private static byte[] CryptoKey
        {
            get
            {
                lock (lockObject)
                {
                    cryptoKey = cryptoKey ?? GetCryptoKey(cryptoKeyName, 24);
                    return cryptoKey;
                }
            }
        }

        /// <summary>
        /// Id of workroom
        /// </summary>
        public PageReference WorkroomStartPage { get; set; }

        /// <summary>
        /// User access level
        /// </summary>
        public MembershipLevels MembershipLevel { get; set; }

        /// <summary>
        /// Email of user
        /// </summary>
        public string Email { get; set; }

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
            var siteConfigDB = new SiteConfigDB();
            var cryptoString = siteConfigDB.GetValue(Settings.Instance.Parent.SiteId, keyName);

            if (!string.IsNullOrEmpty(cryptoString))
            {
                return Convert.FromBase64String(cryptoString);
            }

            var key = new byte[keyLength];
            var rngProvider = new RNGCryptoServiceProvider();
            rngProvider.GetBytes(key);

            siteConfigDB.SetValue(Settings.Instance.Parent.SiteId, keyName, Convert.ToBase64String(key));
            return key;
        }

        /// <summary>
        /// Encrypts invitation to the ticket
        /// </summary>
        /// <returns>Invitation ticket string</returns>
        public string Encrypt()
        {
            var text = WorkroomStartPage.ID.ToString() + separator + (int)MembershipLevel + separator + Email;
            var data = Encoding.UTF8.GetBytes(text);
            var transform = new TripleDESCryptoServiceProvider().CreateEncryptor(CryptoKey, iv);
            var bytes = transform.TransformFinalBlock(data, 0, data.Length);
            var encodingData = Convert.ToBase64String(bytes);
            var bytesValue = Encoding.UTF8.GetBytes(encodingData);
            var base64value = Convert.ToBase64String(bytesValue);
            var result = HttpUtility.UrlEncode(base64value);
            return result;
        }

        /// <summary>
        /// Decrypts invitation from the ticket
        /// </summary>
        /// <param name="ticket">Invitation ticket string</param>
        /// <returns>Invitation object</returns>
        public static InvitationTicket Decrypt(string ticket)
        {
            if (string.IsNullOrEmpty(ticket))
            {
                return null;
            }
            var decodedTicket = HttpUtility.UrlDecode(ticket);
            var bytes64Value = Convert.FromBase64String(decodedTicket);
            var decodingData = new string(Encoding.UTF8.GetChars(bytes64Value));
            var data = Convert.FromBase64String(decodingData);
            var transform = new TripleDESCryptoServiceProvider().CreateDecryptor(CryptoKey, iv);
            var bytes = transform.TransformFinalBlock(data, 0, data.Length);

            var text = Encoding.UTF8.GetString(bytes).Split(separator);
            var workroomStartpage = new PageReference(int.Parse(text[0]));
            var membershipLevel = (MembershipLevels)int.Parse(text[1]);
            var email = text[2];
            return new InvitationTicket(workroomStartpage, membershipLevel, email);
        }
    }
}