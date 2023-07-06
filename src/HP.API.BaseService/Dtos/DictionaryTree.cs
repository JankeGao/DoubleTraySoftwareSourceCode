using System;
using HP.Utility.Data;

namespace HPC.BaseService.Dtos
{
    public class DictionaryTree : TreeBase<DictionaryTree>
    {
        public int Id { set; get; }

        /// <summary>
        /// 编码
        /// </summary>
        public string Code { set; get; }
        /// <summary>
        /// 上级编码
        /// </summary>
        public string ParentCode { set; get; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { set; get; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { set; get; }
        /// <summary>
        /// 值
        /// </summary>
        public string Value { set; get; }
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Enabled { set; get; }
        /// <summary>
        /// 排序
        /// </summary>
        public short Sort { set; get; }
        /// <summary>
        /// 字典分类
        /// </summary>
        public string TypeCode { set; get; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string CreatedUserId { set; get; }

        public string CreatedUserName { set; get; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreatedTime { set; get; }


        /// <summary>
        /// 更新人
        /// </summary>
        public string UpdatedUserId { set; get; }

        public string UpdatedUserName { set; get; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? UpdatedTime { set; get; }
    }
}