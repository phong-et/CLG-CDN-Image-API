﻿using System;
using System.Collections;
using System.IO;
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
            //string origin = "";
            //if (Request.UrlReferrer != null)
            //    origin = Request.UrlReferrer.Host;
            //if (!Utils.IsAllowAccessOrigin((origin)))
            //{
            //    Response.Write(string.Format(responeJson, "false", "Access Deined From: " + (origin == "" ? " Anonymous origin" : origin)));
            //    return;
            //}
            if (token != null)
            {
                if(!token.Contains("Bearer"))
                {
                    Response.Write(string.Format(responeJson, "false", "Token missed out prefix 'Bearer'"));
                    return;
                }
                // Split Bearer text from token
                token = token.Substring(6);
                if (Utils.IsExpiredToken((token)))
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
                    string imageType = (string)hashtable["ImageType"];
                    string folderPath = rootPath + "/" + gameFolder;
                    string fileName = string.Empty;
                    string message = string.Empty;
                    switch (gameFolder)
                    {
                        case "allgames":
                            string gameListID = (string)hashtable["GameListID"];
                            fileName = gameListID + "." + imageType;
                            message = action + " image successfully";
                            switch (action)
                            {
                                case "create":
                                case "update":
                                    Utils.SaveImage(folderPath, fileName, strBase64);
                                    break;
                                case "delete":
                                    File.Delete(folderPath + "/" + fileName);
                                    break;
                            }
                            break;

                        case "lobbygames":
                            fileName = (string)hashtable["GameLobbyId"] + "_" + (string)hashtable["GameCode"] + "." + (string)hashtable["ImageType"];
                            message = action + " " + gameFolder + " image successfully";
                            switch (action)
                            {
                                case "create":
                                case "update":
                                    Utils.SaveImage(folderPath, fileName, strBase64);
                                    break;
                                case "delete":
                                    File.Delete(folderPath + "/" + fileName);
                                    break;
                            }
                            break;

                        case "headergames":
                            string HGameId = (string)hashtable["HGameId"];
                            string GameName = (string)hashtable["GameName"];
                            string GameType = (string)hashtable["GameType"];
                            string ImageType = (string)hashtable["ImageType"];
                            bool isHeaderSubMenuImage = (bool)hashtable["IsHeaderSubMenuImage"];
                            string CTId = (string)hashtable["CTId"];
                            bool isSharedHeaderSubMenuImage = GetIsSharedSubMenuIcon(CTId);
                            if (isHeaderSubMenuImage)
                            {
                                fileName = "SubMenuIcon_" + HGameId + "_" + GameName + "." + ImageType;
                                if(!isSharedHeaderSubMenuImage)
                                {
                                    folderPath += "/" + CTId;
                                    gameFolder += "/" + CTId;
                                }
                            }
                            else
                            {
                                fileName = "MenuIcon_" + GameType + "." + ImageType; ;
                                folderPath += "/" + CTId;
                                gameFolder += "/" + CTId;
                            }
                            message = action + " " + gameFolder + " image successfully";
                            switch (action)
                            {
                                case "create":
                                case "update":
                                    Utils.SaveImage(folderPath, fileName, strBase64);
                                    break;
                                case "delete":
                                    File.Delete (folderPath + "/" + fileName);
                                    break;
                            }
                            break;
                    }
                    responeJson = "{{ \"success\":{0}, \"message\":\"{1}\", \"imagePath\":\"{2}\" }}";
                    string rootUrl = Request.Url.GetLeftPart(UriPartial.Authority) + Request.ApplicationPath.TrimEnd('/');
                    string imagePath = rootUrl + "/" + gameFolder + "/" + fileName;
                    Response.Write(string.Format(responeJson, "true", message, imagePath));
                }
                catch (Exception ex)
                {
                    Response.Write(string.Format(responeJson, false.ToString().ToLower(), ex.ToString()));
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
        private bool GetIsSharedSubMenuIcon(string CTId)
        {
            switch (CTId)
            {
                case "137":
                case "262":
                    return false;
                default: return true;
            }
        }
    }
}