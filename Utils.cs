using System.Collections.Generic;
using System.IO;
using System;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web.Script.Serialization;

namespace cdn
{
    public class Utils
    {
        public static bool IsAllowAccessOrigin(string origin)
        {
            // Prevent POSTMAN anonymous
            if (origin == "") 
                return false;
            // Only Allow Web Request and whitelist host
            var allowedDomains = System.Configuration.ConfigurationManager.AppSettings["AllowedDomains"];
            return allowedDomains.Contains(origin);
        }
        public static IPAddress[] IsAllowAccessIP(string origin)
        {
            IPAddress[] ips = Dns.GetHostAddresses("");
            return ips;
        }
        public static bool isValidToken(string token)
        {

            return true;
        }

        public static bool isRejectedToken(string token)
        {
            switch (token)
            {
                case "token1": 
                    return true;
            }
            return false;
        }

        private static string aseTokenSecretKey = "aA123Bb321@8*iPh";
        #region Utility Funtions
        public static string GetFileContent(string fileName, string token)
        {
            var fileContent = "";
            if (token == "" || token == null)
                fileContent = "Token not found";
            else if (!isExpiredToken(token))
                if (File.Exists(fileName))
                    fileContent = File.ReadAllText(fileName, Encoding.UTF8);
                else fileContent = "File Not Found";
            else fileContent = "Token is expired";
            return fileContent;
        }
        private static bool isExpiredToken(string token)
        {
            try
            {
                var decryptedToken = DecryptStringFromBytes_Aes(Convert.FromBase64String(token));
                var o = Serialize(decryptedToken);
                return ((long)o["expiredDate"] - GetTimeNow() < 0);
            }
            catch (Exception e)
            {
                return true;
            }
        }
        public static long GetTimeNow() => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        public static long GetTime(int dayQuantity = 0)
        {
            if(dayQuantity == 0)
                return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            DateTime now = DateTime.Now; 
            DateTime nextDays = now.AddDays(dayQuantity);
            TimeZoneInfo localTimeZone = TimeZoneInfo.Local;
            DateTimeOffset utcDateTimeOffset = new DateTimeOffset(nextDays, localTimeZone.GetUtcOffset(nextDays));
            return utcDateTimeOffset.ToUnixTimeMilliseconds();
        }
        #endregion

        public static Dictionary<string, object> Serialize(string json) => (Dictionary<string, object>)new JavaScriptSerializer().DeserializeObject(json);
        #region ASE Members
        public static string EncryptStringToStrings_Aes(string plainText)
        {
            byte[] Key = Encoding.UTF8.GetBytes(aseTokenSecretKey);
            byte[] IV = Encoding.UTF8.GetBytes(aseTokenSecretKey);
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");
            byte[] encrypted;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;
                aesAlg.Mode = CipherMode.CBC;
                aesAlg.Padding = PaddingMode.PKCS7;
                aesAlg.FeedbackSize = 128;

                // Create an encryptor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.

                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            // Return the encrypted bytes from the memory stream.
            return Convert.ToBase64String(encrypted);
            //return Convert.ToString(encrypted);
        }
        public static string DecryptStringFromBytes_Aes(byte[] cipherText)
        {
            byte[] Key = Encoding.UTF8.GetBytes(aseTokenSecretKey);
            byte[] IV = Encoding.UTF8.GetBytes(aseTokenSecretKey);

            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;
                aesAlg.Mode = CipherMode.CBC;
                aesAlg.Padding = PaddingMode.PKCS7;
                aesAlg.FeedbackSize = 128;
                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {

                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return plaintext;
        }
        #endregion
    }
}
