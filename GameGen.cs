using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace cdn
{
    public class GameGen
    {
        private static int cacheTime = 60;
        public static string GetLastDataJsonLOBBYGAMES()
        {
            try
            {
                DataSet ds;
                DataTable dt;
                string jsonTemplate = "{{'success':{0}, 'text':'{1}', 'games':{2}}}";
                string jsonStr = "";
                ds = Common.GetDataSetCache("_cmListGameLobby_sw", null, "ListLobbyGame", Common.ConnStr, cacheTime);
                dt = ds.Tables[0];
                if (dt.Rows.Count > 0)
                    jsonStr = Common.ConvertDataTableToJson(dt);
                else
                    return (String.Format(jsonTemplate, "false", "_cmListGameLobby_sw sp has not data ", "[]").Replace("'", "\""));
                return (string.Format(jsonTemplate, "true", "", jsonStr).Replace("'", "\""));
            }
            catch (Exception ex)
            {
                return ("{\"success\":" + "false" + ", \"text\":\"" + ex.Message.Replace("\n", " ") + "\"}");
            }
        }
        public static string GetLastDataJsonHEADERGAMES(int CTId = 0)
        {
            try
            {
                DataSet ds;
                DataTable dt;
                string jsonTemplate = "{{'success':{0}, 'text':'{1}', 'menus':{2}, 'submenuIcons': {3} }}";
                string jsonStrMainMenu = "";
                string jsonStrSubMenuIcons = "";
                ds = Common.GetDataSetCache("_cmListHeaderGame_sw",
                    CTId == 0 ? null : new List<SqlParameter> { new SqlParameter("@CTId", SqlDbType.Int) { Value = CTId } },
                    "LisMenutHeaderGames", Common.ConnStr, 1000);
                dt = ds.Tables[0];
                Dictionary<string, string> subMenuIconMap = new Dictionary<string, string>();
                if (dt.Rows.Count > 0)
                {
                    jsonStrMainMenu = Common.ConvertDataTableToJson(dt);
                    ds = Common.GetDataSetCache("_cmListAllHeaderSubMenuGameIcon_sw", null, "_cmListAllHeaderSubMenuGameIcon_sw", Common.ConnStr, cacheTime);
                    dt = ds.Tables[0];
                    if (dt.Rows.Count > 0)
                        for (int i = 0; i < dt.Rows.Count; i++)
                            subMenuIconMap.Add(dt.Rows[i]["HGameId"].ToString(), (string)dt.Rows[i]["GameIcon"]);
                    jsonStrSubMenuIcons = new JavaScriptSerializer().Serialize(subMenuIconMap);
                }
                else
                {
                    return (String.Format(jsonTemplate, "false", "_cmListHeaderGame_sw sp has not data", "[]").Replace("'", "\""));
                }

                return (string.Format(jsonTemplate, "true", "", jsonStrMainMenu, jsonStrSubMenuIcons).Replace("'", "\""));
            }
            catch (Exception ex)
            {
                return ("{\"success\":" + "false" + ", \"text\":\"" + ex.Message.Replace("\n", " ") + "\"}");
            }
        }
        public static string GetLastDataJsonALLGAMES()
        {
            try
            {
                DataSet ds;
                DataTable dt;
                string jsonTemplate = "{{'success':{0},'data':{1}}}";
                string jsonStr = "";
                ds = Common.GetDataSetCache("_cmListAllGameImage_sw", null, "_cmListAllGameImage_sw", Common.ConnStr, cacheTime);
                dt = ds.Tables[0];
                if (dt.Rows.Count > 0)
                    jsonStr = Common.ConvertDataTableToJson(dt);
                else
                {
                    return (String.Format(jsonTemplate, "false", "_cmListHeaderGame_sw sp has not data").Replace("'", "\""));
                }
                return (string.Format(jsonTemplate, "true", jsonStr).Replace("'", "\""));
            }
            catch (Exception ex)
            {
                return ("{\"success\":" + "false" + ", \"text\":\"" + ex.Message.Replace("\n", " ") + "\"}");
            }
        }
    }
}