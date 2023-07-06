using System;
using HP.Utility.Data;

namespace HPC.BaseService.Dtos
{
    public class DepartmentTree:TreeBase<DepartmentTree>
    {
        public int Id { set; get; }
        /// <summary>
        /// 编码
        /// </summary>
        public string Code { set; get; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { set; get; }
        /// <summary>
        /// 简称
        /// </summary>
        public string ShortName { set; get; }
        /// <summary>
        /// 部门性质
        /// </summary>
        public string NatureDict { set; get; }
        /// <summary>
        /// 联系电话
        /// </summary>
        public string Telephone { set; get; }
        /// <summary>
        /// 分机号
        /// </summary>
        public string ENumber { set; get; }
        /// <summary>
        /// 上级编码
        /// </summary>
        public string ParentCode { set; get; }
        /// <summary>
        /// 机构编码
        /// </summary>
        public string OrganizationCode { set; get; }
        /// <summary>
        /// 机构名称
        /// </summary>
        public string OrganizationName { set; get; }
        /// <summary>
        /// 组织机构Flag
        /// </summary>
        public string OrganizationFlag { set; get; }
        /// <summary>
        /// 标志
        /// </summary>
        public string Flag { set; get; }
        /// <summary>
        /// 部门负责人
        /// </summary>
        public string Manager { set; get; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { set; get; }
        /// <summary>
        /// 创建人
        /// </summary>
        public string CreatedUserCode { set; get; }
        public string CreatedUserName { set; get; }
        public DateTime CreatedTime { set; get; }
        /// <summary>
        /// 更新人
        /// </summary>
        public string UpdatedUserCode { set; get; }
        public string UpdatedUserName { set; get; }
        public DateTime? UpdatedTime { set; get; }
    }
}
