using System;
using HP.Data.Entity.Configs;

namespace wms.Client
{
    public partial class DbContextConfig : DbContextConfigBase
    {
        private const string DefaultConnectionStringName = "Default";

        //private string connectionString = getConnectionString();

        //private string getConnectionString()
        //{
        //    string url = HttpContext.Current.Request.Url.Host;
        //    int startIndex = url.IndexOf("."); //开始位置
        //    string connectionString = url.Substring(0, startIndex);
        //    return connectionString;
        //}

        public DbContextConfig() : base(DefaultConnectionStringName)
        {
            DbConfigurationAppend();
        }

        //public DbContextConfig() : base(DefaultConnectionStringName)
        //{
        //    DbConfigurationAppend();
        //}

        public override Type DbContextConfigType
        {
            get
            {
                return this.GetType();
            }
        }

        /// <summary>
        /// 额外的数据映射
        /// </summary>
        partial void DbConfigurationAppend();
    }
}