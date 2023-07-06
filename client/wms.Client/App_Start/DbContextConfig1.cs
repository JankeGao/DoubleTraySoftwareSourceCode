using System;
using HP.Data.Entity.Configs;
using HPC.BaseService.Models;

namespace wms.Client
{
    public partial class DbContextConfig1 : DbContextConfigBase
    {
        private const string DefaultConnectionStringName = "Default1";

        public DbContextConfig1() : base(DefaultConnectionStringName)
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
                return new string[] { typeof(ToDo).FullName };
            }
        }

        /// <summary>
        /// 额外的数据映射
        /// </summary>
        partial void DbConfigurationAppend();
    }
}