using System;
using HP.Data.Entity.Configs;
using System.Web;

namespace DF.Web
{
    public partial class DbContextConfig : DbContextConfigBase
    {
        private const string DefaultConnectionStringName = "Default";

        public DbContextConfig() : base(DefaultConnectionStringName)
        {
            DbConfigurationAppend();
        }

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