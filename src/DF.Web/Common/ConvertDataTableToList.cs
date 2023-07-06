using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace DF.Web.Common
{
   public class ConvertDataTableToList<T> where T:new() 
    {
        public static IList<T> ConvertToModel(DataTable dt)
        {

            IList<T> ts = new List<T>();// 定义集合
            Type type = typeof(T); // 获得此模型的类型
            string tempName = "";
            foreach (DataRow dr in dt.Rows)
            {
                T t = new T();
                PropertyInfo[] propertys = t.GetType().GetProperties();// 获得此模型的公共属性
                foreach (PropertyInfo pi in propertys)
                {
                    tempName = pi.Name;
                    if (dt.Columns.Contains(tempName))
                    {
                        if (!pi.CanWrite) continue;
                        object value = dr[tempName];
                        if (value != DBNull.Value)
                            pi.SetValue(t, value, null);
                    }
                }
                ts.Add(t);
            }
            return ts;
        }

        public static T GetModelByDataRow<T>(System.Data.DataRow dr) where T : new()
        {
            T model = new T();
            foreach (PropertyInfo pInfo in model.GetType().GetProperties())
            {
                object val = getValueByColumnName(dr, pInfo.Name);
                pInfo.SetValue(model, val, null);
            }
            return model;
        }
        //返回DataRow 中对应的列的值。  
        public static object getValueByColumnName(System.Data.DataRow dr, string columnName)
        {
            if (dr.Table.Columns.IndexOf(columnName) >= 0)
            {
                if (dr[columnName] == DBNull.Value)
                    return null;
                return dr[columnName];
            }
            return null;
        }

        public static T GetEntityByDataRow(System.Data.DataRow dr)
        {
            T model = new T();
            Type type = typeof(T);
            string tempName = "";
            PropertyInfo[] propertys = model.GetType().GetProperties();
            foreach (PropertyInfo pi in propertys)
            {
                tempName = pi.Name;
                if (dr.Table.Columns.Contains(tempName))
                {
                    if (!pi.CanWrite) continue;
                    object value = dr[tempName];
                    if (value != DBNull.Value)
                        pi.SetValue(model, value, null);
                }
            }
            return model;
        }
    }
}
