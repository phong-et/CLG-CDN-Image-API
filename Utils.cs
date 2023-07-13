using System.Collections.Generic;
using System.IO;
using System;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web.Script.Serialization;
using System.Configuration;
using System.Web;

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
            var allowedDomains = ConfigurationManager.AppSettings["AllowedDomains"];
            return allowedDomains.Contains(origin);
        }
        public static IPAddress[] IsAllowAccessIP(string origin)
        {
            IPAddress[] ips = Dns.GetHostAddresses("");
            return ips;
        }
        public static string GetMd5Hash(string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    builder.Append(hashBytes[i].ToString("x2"));
                }

                return builder.ToString();
            }
        }
        public static bool IsRejectedToken(string token)
        {
            switch (token)
            {
                case "token1": 
                    return true;
            }
            return false;
        }
        public static bool IsExpiredToken(string token)
        {
            string decryptedToken = TokenHandler.decrypt(token);
            Dictionary<string, object> objToken = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(decryptedToken);
            return ((long)objToken["expiredTime"] - Utils.GetTime()) < 0;
        }
        public static bool IsExpiredTokenSync(string token)
        {
            string decryptedToken = TokenHandler.decrypt(token);
            Dictionary<string, object> objToken = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(decryptedToken);
            return ((long)objToken["expiredTime"] - Utils.GetTime()) < 0;
        }
        public static string aseTokenSecretKey = (string)ConfigurationManager.AppSettings["secretKeyASE"];
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
        public static Dictionary<string, object> Serialize(string json) => (Dictionary<string, object>)new JavaScriptSerializer().DeserializeObject(json);
        #region ASE Members
        public static string EncryptStringToStrings_Aes(string plainText, string aseTokenSecretKey)
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
        public static string DecryptStringFromBytes_Aes(byte[] cipherText, string aseTokenSecretKey)
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
        public static void SaveImage(string folderPath, string fileName, string strBase64)
        {
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);
            File.WriteAllBytes(folderPath + "/" + fileName, Convert.FromBase64String(strBase64));
        }
        public static string convetTimeStampToString(long timeStamp)
        {
            DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeMilliseconds(timeStamp);
            DateTime dateTime = dateTimeOffset.LocalDateTime;
            string strDate = dateTime.ToString("dddd, MMMM d, yyyy h:mm tt");
            return strDate;
        }
        public static void writeLogs(string message, string fileName = "log.txt")
        {
            string logLine = $"{convetTimeStampToString(Utils.GetTimeNow())} : {message}";

            using (StreamWriter writer = File.AppendText(HttpContext.Current.Server.MapPath("~") + "/" + fileName))
            {
                writer.WriteLine(logLine);
            }
        }
    }
}
