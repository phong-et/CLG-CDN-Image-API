using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.SessionState;

namespace cdn
{
    /// <summary>
    /// Summary description for ImageHandler
    /// </summary>
    public class ImageHandler : IHttpHandler, IReadOnlySessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            string responeJson = "{{ \"success\":{0}, \"message\":\"{1}\" }}";
            context.Response.AppendHeader("Access-Control-Allow-Headers", "Authorization");
            HttpResponse Response = context.Response;
            HttpRequest Request = context.Request;
            Response.ContentType = "application/json";
            string token = Request.Headers["Authorization"];
            string origin = "";
            if (Request.UrlReferrer != null)
                origin = Request.UrlReferrer.Host;
            if (!Utils.IsAllowAccessOrigin((origin)))
            {
                Response.Write(string.Format(responeJson, false.ToString().ToLower(), "Access Deined"));
                return;
            }
            if (token != null)
            {
                // Split Bearer text from token
                token = token.Substring(6);
                if (Utils.isExpiredToken((token)))
                {
                    Response.Write(string.Format(responeJson, false.ToString().ToLower(), "Token is expired"));
                    return;
                }
            }
            else
            {
                Response.Write(string.Format(responeJson, false.ToString().ToLower(), "Token is required at Header Authorization"));
                return;
            }
            using (var sr = new StreamReader(Request.InputStream))
            {
                string body = sr.ReadToEnd();
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                Hashtable hashtable = serializer.Deserialize<Hashtable>(body);
                string[] segments = new Uri(Request.Url.ToString()).AbsolutePath.Trim('/').Split('/');
                var gameFolder = segments[segments.Length - 2];
                var action = segments[segments.Length - 1];
                try
                {
                    string rootPath = context.Server.MapPath("~");
                    string strBase64 = (string)hashtable["strBase64"];
                    string imageType = (string)hashtable["imageType"];
                    string folderPath = rootPath + "/" + gameFolder;
                    string fileName = string.Empty;
                    string message = string.Empty;
                    switch (gameFolder)
                    {
                        case "allgames":
                            string gameListID = (string)hashtable["gameListID"];
                            fileName = gameListID + "." + imageType;
                            message = action + " image successfully";
                            switch (action)
                            {
                                case "create":
                                case "update":
                                    SaveImage(folderPath, fileName, strBase64);
                                    break;
                                case "delete":
                                    File.Delete(folderPath + "/" + fileName);
                                    break;
                            }
                            break;

                        case "lobbygames":
                            fileName = (string)hashtable["CTId"] + "_" + (string)hashtable["GameLobbyId"] + "_" + (string)hashtable["GameCode"] + "." + (string)hashtable["ImageType"];
                            message = action + " " + gameFolder + " image successfully";
                            switch (action)
                            {
                                case "create":
                                case "update":
                                    SaveImage(folderPath, fileName, strBase64);
                                    break;
                                case "delete":
                                    File.Delete(folderPath + "/" + fileName);
                                    break;
                            }
                            break;

                        case "headergames":
                            fileName = (string)hashtable["Id"] + "." + (string)hashtable["ImageType"];
                            bool isSharedIcon = (bool)hashtable["IsSharedIcon"];
                            if (isSharedIcon) folderPath += "/" + (string)hashtable["CTId"];
                            message = action + " " + gameFolder + " image successfully";
                            switch (action)
                            {
                                case "create":
                                case "update":
                                    SaveImage(folderPath, fileName, strBase64);
                                    break;
                                case "delete":
                                    File.Delete (folderPath + "/" + fileName);
                                    break;
                            }
                            break;
                    }
                    Response.Write(string.Format(responeJson, true.ToString().ToLower(), message));
                }
                catch (Exception ex)
                {
                    Response.Write(string.Format(responeJson, false.ToString().ToLower(), ex.ToString()));
                }
            }
        }
        private void SaveImage(string folderPath, string fileName, string strBase64)
        {
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);
            File.WriteAllBytes(folderPath + "/" + fileName, Convert.FromBase64String(strBase64));
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