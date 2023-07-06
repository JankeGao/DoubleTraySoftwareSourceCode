using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Data.SqlClient;

namespace DF.Web.Areas.BussinessApi.Controllers
{
    /// <summary>
    /// 获取tb_wms_in表中的数据
    /// </summary>
    public class CRRCDataController : ApiController
    {
        /// <summary>
        /// 获取tb_wms_in表中的数据
        /// </summary>
        /// <returns></returns>
        // GET api/wmsin
        public IEnumerable<string> Get()
        {
            // TODO: 连接数据库，查询tb_wms_in表中的数据，并将数据转换为JSON格式返回
            string connectionString = "Data Source=DESKTOP-71I0RDA;Initial Catalog=SMDB;User ID=sa;Password=123456";
            string queryString = "SELECT * FROM View_ConnectionTest";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        yield return reader.GetString(0);
                    }
                }
                finally
                {
                    reader.Close();
                }
            }
        }
    }
}
