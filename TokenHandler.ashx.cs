﻿using System;
using System.Configuration;
using System.IO;
using System.Web;
using System.Web.SessionState;

namespace cdn
{
    /// <summary>
    /// Summary description for TokenHandler
    /// </summary>
    /// 
    public class TokenHandler : IHttpHandler, IReadOnlySessionState
    {
        public static string decrypt(string token)
        {
            return Utils.DecryptStringFromBytes_Aes(Convert.FromBase64String(token), Utils.aseTokenSecretKey);
        }
        
        public void ProcessRequest(HttpContext context)
        {
            string responeJson = "{{ \"success\":{0}, \"message\":\"{1}\", \"token\":\"{2}\" }}";
            HttpResponse Response = context.Response;
            HttpRequest Request = context.Request;
            Response.ContentType = "application/json";
            //string origin = "";
            //if (Request.UrlReferrer != null)
            //    origin = Request.UrlReferrer.Host;
            //if (!Utils.IsAllowAccessOrigin((origin)))
            //{
            //    Response.Write(string.Format(responeJson, "fasle", "Access Deined From:" + (origin == "" ?" Anonymous origin": origin), ""));
            //    return;
            //}
            using (var sr = new StreamReader(Request.InputStream))
            {
                try
                {
                    string[] segments = new Uri(Request.Url.ToString()).AbsolutePath.Trim('/').Split('/');
                    var action = segments[segments.Length - 1];
                    string secretKey = (string)Request.Form["secretKey"];
                    string token = string.Empty;
                    if (Utils.GetMd5Hash(secretKey) == (string)ConfigurationManager.AppSettings["secretKeyCreateToken"])
                    {
                        switch (action)
                        {
                            case "create":
                                int dayQuantity = int.TryParse(Request.Form["days"], out int parsedResult) ? parsedResult : 1;
                                string tokenTemplate = "{{ \"expiredTime\":{0}, \"expiredDate\":\"{1}\" }}";
                                long expiredTime = Utils.GetTime(dayQuantity);
                                string expiredDate = Utils.convetTimeStampToString(expiredTime);
                                var plainToken = string.Format(tokenTemplate, expiredTime, expiredDate);
                                token = Utils.EncryptStringToStrings_Aes(plainToken, Utils.aseTokenSecretKey);
                                Utils.writeLogs(token);
                                Response.Write(string.Format(responeJson, "true", "create token successfully", token));
                                break;
                            case "decrypt":
                                token = (string)Request.Form["token"];
                                string decryptedToken = decrypt(token);
                                responeJson = "{{ \"success\":{0}, \"message\":\"{1}\", \"token\":{2} }}";
                                Response.Write(string.Format(responeJson, "true", "decrypt token successfully", decryptedToken));
                                break;
                            case "check":
                                token = (string)Request.Form["token"];
                                bool isExpired = Utils.IsExpiredToken(token);
                                responeJson = "{{ \"expired\":{0}, \"message\":\"{1}\" }}";
                                Response.Write(string.Format(responeJson, isExpired.ToString().ToLower(), "token has " + (isExpired ? "": "not ") + "expried yet"));
                                break;
                        }
                    }
                    else 
                        Response.Write(string.Format(responeJson, "false", "secret key is wrong", ""));
                }
                catch (Exception ex)
                {
                    Response.Write(string.Format(responeJson, "false", ex.ToString(), ""));
                }
            }
        }
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}
