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
            string responeJson = "{{ \"success\":{0}, \"message\":\"{1}\" }}";
            context.Response.AppendHeader("Access-Control-Allow-Headers", "Authorization");
            HttpResponse Response = context.Response;
            HttpRequest Request = context.Request;
            Response.ContentType = "application/json";
            string token = Request.Headers["Authorization"];
            if (token != null)
            {
                if (!token.Contains("Basic"))
                {
                    Response.Write(string.Format(responeJson, "false", "Token missed out prefix 'Basic'"));
                    return;
                }
                // Split Basic from Support page
                token = token.Substring(5);
                if (IsExpiredToken((token)))
                {
                    Response.Write(string.Format(responeJson, "false", "Token is expired"));
                    return;
                }
            }
            else
            {
                Response.Write(string.Format(responeJson, "false", "Token is required at Header Authorization"));
                return;
            }
            string[] segments = new Uri(Request.Url.ToString()).AbsolutePath.Trim('/').Split('/');
            var gameFolder = segments[segments.Length - 1];
            string mbUrl = string.Empty;
            using (var sr = new StreamReader(Request.InputStream))
            {
                string body = sr.ReadToEnd();
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                Hashtable hashtable = serializer.Deserialize<Hashtable>(body);
                mbUrl = Uri.UnescapeDataString((string)hashtable["mbUrl"]);
            }
            
            mbUrl += "/public/GameGen.ashx?cmd=GetLastDataJson" + gameFolder.ToUpper();
            try
            {
                sendRequest(mbUrl, gameFolder);
                string message = $"create {gameFolder}.json successfully";
                Response.Write(string.Format(responeJson, "true", message));
            }
            catch (Exception ex)
            {
                Response.Write(string.Format(responeJson, "false", ex.ToString()));
            }
        }
        private void sendRequest(string mbUrl, string gameFolder)
        {
            //Utils.writeLogs(mbUrl, "synclog.txt");
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(mbUrl);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "GET";
            // receive respose
            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                // return a json string
                var jsonResult = streamReader.ReadToEnd();
                using (StreamWriter writer = new StreamWriter(HttpContext.Current.Server.MapPath("~") + "/" + gameFolder + ".json"))
                {
                    writer.Write(jsonResult);
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
    }
}