using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data.SqlClient;
using System.Threading;
using Bussiness.Entitys;
using HP.Core.Data;
using System.Web;

namespace Bussiness.Services
{
    public class CRRCDataContServer 
    {

        public static void CRRCDataConnect()
        {
            var connectionString = "Data Source=DESKTOP-71I0RDA;Initial Catalog=SMDB;User ID=sa;Password=123456";
            var query = "SELECT * FROM View_ConnectionTest";
            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand(query, connection);
                connection.Open();
                var reader = command.ExecuteReader();
                var resultList = new List<Dictionary<string, object>>();
                while (reader.Read())
                {
                    var row = new Dictionary<string, object>();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        row.Add(reader.GetName(i), reader.GetValue(i));
                    }
                    resultList.Add(row);
                }
                reader.Close();

                List<In> inList = new List<In>();
                foreach (var dict in resultList)
                {
                    In inEnity = new In
                    {
                        Id = 0,
                        BillCode = "",
                        Code = "",
                        Remark = "",
                        WareHouseCode = dict["WareHouseCode"].ToString(),
                        AddMaterial = new List<InMaterial>(),
 
                    };


                    InMaterial inMaterialObj = new InMaterial
                    {
                        Id = (int)dict["Id"],
                        MaterialCode = dict["MaterialCode"].ToString(),
                        Quantity = (decimal)dict["Quantity"],
                        ManufactrueDate = (DateTime?)dict["ManufactrueDate"],

                        
                        Status = 0,
                        RealInQuantity = 0,
                        
                        BatchCode = "",
                        

                    };

                    inEnity.AddMaterial.Add(inMaterialObj);
                    inList.Add(inEnity);
                    
                }

                InServer inServer = new InServer();
                foreach (var inEntity in inList)
                {
                    inServer.CreateInEntity(inEntity);
                }

            }
        }
    }
}
