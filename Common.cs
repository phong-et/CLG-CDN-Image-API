

using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Web;
using System;
using System.Web.Script.Serialization;

namespace cdn
{
    public static class Common
    {

        public static DataSet GetDataSetCache(string sp, List<SqlParameter> sqlParams, string cacheName, string connStr, int cacheTime = 60)
        {
            if (HttpContext.Current.Cache[cacheName] == null)
            {
                HttpContext.Current.Cache.Insert(cacheName, GetDataSet(sp, sqlParams, connStr), null, DateTime.Now.AddSeconds(cacheTime), TimeSpan.Zero);
            }

            return (DataSet)HttpContext.Current.Cache[cacheName];
        }
        public static DataSet GetDataSet(string sp, List<SqlParameter> sqlParams, string connStr)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(sp, con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 300;
                        if (sqlParams != null)
                        {
                            foreach (IDataParameter para in sqlParams)
                            {
                                cmd.Parameters.Add(para);
                            }
                        }
                        using (SqlDataAdapter da = new SqlDataAdapter())
                        {
                            da.SelectCommand = cmd;
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                con.Close();
                                return ds;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public static string ConnStr = Cryptography.Decrypt(System.Configuration.ConfigurationManager.AppSettings["ConnStr"]);
        public static string ConvertDataTableToJson(DataTable dt)
        {
            try
            {
                string jsonStr = "";
                JavaScriptSerializer serializer;
                if (dt.Rows.Count > 0)
                {
                    serializer = new JavaScriptSerializer();
                    serializer.MaxJsonLength = Int32.MaxValue;
                    List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
                    Dictionary<string, object> row;
                    foreach (DataRow dr in dt.Rows)
                    {
                        row = new Dictionary<string, object>();
                        foreach (DataColumn col in dt.Columns)
                            row.Add(col.ColumnName, dr[col]);
                        rows.Add(row);
                    }
                    jsonStr = serializer.Serialize(rows);
                }
                else
                    jsonStr = "[]";
                return jsonStr;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}