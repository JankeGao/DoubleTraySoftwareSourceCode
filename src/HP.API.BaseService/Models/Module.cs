using System.ComponentModel;
using HP.Core.Data;
using HP.Data.Orm.Entity;

namespace HPC.BaseService.Models
{
    [Description("模块信息")]
    [Table("Base_Module")]
    public class Module : EntityBase<int>
    {
        [Sequence("Seq_Module")]
        public override int Id
        {
            set { base.Id = value; }
            get { return base.Id; }
        }

        //public Module()
        //{
        //    Enabled = true;
        //    Target = ModuleTarget.Page.ToString();
        //    AuthType = AuthTypeEnum.Authorization.ToString();
        //}

        /// <summary>
        /// 模块编码
        /// </summary>
        public string Code { set; get; }
        /// <summary>
        /// 模块名称
        /// </summary>
        public string Name { set; get; }

        /// <summary>
        /// 父亲模块ID
        /// </summary>
        public string ParentCode { set; get; }

        /// <summary>
        /// 模块URL地址
        /// </summary>
        public string Address { set; get; }

        /// <summary>
        /// 模块图标
        /// </summary>
        public string Icon { set; get; }

        /// <summary>
        /// 模块排序
        /// </summary>
        public int Sort { set; get; }

        /// <summary>
        /// 模块备注
        /// </summary>
        public string Remark { set; get; }

        /// <summary>
        /// 模块是否启用
        /// </summary>
        public bool Enabled { set; get; }

        /// <summary>
        /// 目标
        /// </summary>
        public string Target { set; get; }
        /// <summary>
        /// 授权类型
        /// </summary>
        public string AuthType { set; get; }
        /// <summary>
        /// 模块类型
        /// </summary>
        public string Type { set; get; }

        /// <summary>
        /// 区域
        /// </summary>
        public string Area { set; get; }
        /// <summary>
        /// 控制器
        /// </summary>
        public string Controller { set; get; }
    }
}
