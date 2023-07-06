using System;
using DF.Web.Models;
using HP.Data.Entity.Configs;

namespace DF.Web
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