using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.DynamicData;
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
                    "LisMenutHeaderGames_" + CTId, Common.ConnStr);
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
        public static string GetLastDataJsonALLGAMES(int CTId = 0)
        {
            DataSet ds;
            DataTable dt;
            string jsonTemplate = "{{'success':{0}, 'games':{1}, 'images':{2} }}";
            try
            {
                string jsonStrGames = "";
                string jsonStrImages = "[]";
                ds = Common.GetDataSetCache("_cmListHomePageGameByCT_sw", 
                    CTId == 0 ? null : new List<SqlParameter> { new SqlParameter("@CTId", SqlDbType.Int) { Value = CTId }}, "cmListHomePageGameByCT_sw" + CTId, 
                    Common.ConnStr,
                    3600);
                dt = ds.Tables[0];
                if (dt.Rows.Count > 0)
                {
                    jsonStrGames = Common.ConvertDataTableToJson(dt);
                    var gameListIDs = dt.AsEnumerable().Select(row => row.Field<int>("GameListID"));

                    Dictionary<string, string> whiteLabelMapImages = new Dictionary<string, string>();
                    Dictionary<string, string> globalMapImages = new Dictionary<string, string>();
                    ds = Common.GetDataSetCache("_cmListAllGameImage_sw", null, "cmListAllGameImage_sw", Common.ConnStr, 3600);
                    foreach (DataRow dr in ds.Tables[0].Rows)
                        globalMapImages.Add(dr["GameListID"].ToString(), dr["imgBase64Str"].ToString());
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    serializer.MaxJsonLength = Int32.MaxValue;
                    foreach (var id in gameListIDs)
                        whiteLabelMapImages[id.ToString()] = globalMapImages[id.ToString()].ToString();
                    jsonStrImages = serializer.Serialize(whiteLabelMapImages);
                    return string.Format(jsonTemplate, "true", jsonStrGames, jsonStrImages).Replace("'", "\"");
                }
                else
                    return String.Format(jsonTemplate, "false", "_cmListHomePageGameByCT_sw sp has not data").Replace("'", "\"");
            }
            catch (Exception ex)
            {
                return string.Format("{{'success':{0}, 'message':{1} }}", "false", ex.Message.Replace("\n", " ")).Replace("'", "\"");
            }
        }
    }
}