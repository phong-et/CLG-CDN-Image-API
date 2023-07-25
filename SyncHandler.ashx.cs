using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Web;
using System.Web.Script.Serialization;

namespace cdn
{
    /// <summary>
    /// Summary description for Sync
    /// </summary>
    public class SyncHandler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string responseJson = "{{ \"success\":{0}, \"message\":\"{1}\" }}";
            context.Response.AppendHeader("Access-Control-Allow-Headers", "Authorization");
            HttpResponse Response = context.Response;
            HttpRequest Request = context.Request;
            Response.ContentType = "application/json";
            string token = Request.Headers["Authorization"];
            //if(!IsValidToken(token, responseJson, Response)) return;
            string[] segments = new Uri(Request.Url.ToString()).AbsolutePath.Trim('/').Split('/');
            var gameFolder = segments[segments.Length - 1];
            int CTId = int.TryParse(context.Request.QueryString["CTId"], out int result) ? result : 0;
            try
            {
                switch (gameFolder)
                {
                    case "allgames":
                        Response.Write(GameGen.GetLastDataJsonALLGAMES(CTId));
                        break;
                    case "headergames":
                        Response.Write(GameGen.GetLastDataJsonHEADERGAMES(CTId));
                        break;
                    case "lobbygames":
                        Response.Write(GameGen.GetLastDataJsonLOBBYGAMES());
                        break;
                }
                //Response.Write(string.Format(responeJson, "true", CTId));
            }
            catch (Exception ex)
            {
                Response.Write(string.Format(responseJson, "false", ex.ToString()));
            }
        }
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
        /* The token bosite is difference token support site 
         * This function use for checking token from support page 
         * So the algorithm is belong to maintenance project code
         * I will copy code from that project to here
         * 
         */
        private bool IsExpiredToken(string token)
        {
            try
            {
                double GetTimeNow = DateTime.Now.Subtract(new DateTime(1970, 1, 1).ToUniversalTime()).TotalMilliseconds;
                var decryptedToken = Utils.DecryptStringFromBytes_Aes(Convert.FromBase64String(token), "aA123Bb321@8*iPg");
                var o = Utils.Serialize(decryptedToken);
                return ((long)o["expiredDate"] - GetTimeNow < 0);
            }
            catch (Exception e)
            {
                return false;
            }
        }
        private bool IsValidToken(string token, string responseJson, HttpResponse Response)
        {
            if (token != null)
            {
                if (!token.Contains("Basic"))
                {
                    Response.Write(string.Format(responseJson, "false", "Token missed out prefix 'Basic'"));
                    return false;
                }
                // Split Basic from Support page
                token = token.Substring(5);
                if (IsExpiredToken((token)))
                {
                    Response.Write(string.Format(responseJson, "false", "Token is expired"));
                    return false;
                }
            }
            else
            {
                Response.Write(string.Format(responseJson, "false", "Token is required at Header Authorization"));
                return false;
            }
            return true;
        }
    }
}