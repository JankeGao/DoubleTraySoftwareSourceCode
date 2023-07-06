using System;
using HP.Data.Entity.Configs;

namespace DF.Web
{
    public partial class OracleDbContextConfig : DbContextConfigBase
    {
        private const string DefaultConnectionStringName = "Oracle";

        public OracleDbContextConfig() : base(DefaultConnectionStringName)
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

        public override string[] EntityMappings
        {
            get
            {
                //我是Oracle数据库中的ToDo实体
                string[] entities = { typeof(Models.ToDo).FullName };
                return entities;
            }
        }

        /// <summary>
        /// 额外的数据映射
        /// </summary>
        partial void DbConfigurationAppend();
    }
}