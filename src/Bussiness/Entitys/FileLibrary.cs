using System;
using System.ComponentModel;
using HP.Core.Data;
using HP.Core.Data.Infrastructure;
using HP.Data.Orm.Entity;

namespace Bussiness.Entitys
{
    /// <summary>
    /// 文件库
    /// </summary>
    [Table("TB_WMS_FileLibrary")]
    [Description("文件库")]
    public class FileLibrary : ServiceEntityBase<int>
    {
        /// <summary>
        /// 文件库类别编码
        /// </summary>
        public int CategoryId { set; get; }

        /// <summary>
        /// 文件库类别编码
        /// </summary>
        public string Code { set; get; }

        /// <summary>
        /// 文件名称
        /// </summary>
        public string FileName { set; get; }

        /// <summary>
        /// 扩展名
        /// </summary>
        public string ExtensionName { set; get; }

        /// <summary>
        /// MIME内容类型
        /// </summary>
        public string ContentType { set; get; }

        /// <summary>
        /// 图片大小
        /// </summary>
        public int Size { set; get; }

        /// <summary>
        /// 路径
        /// </summary>
        public string FilePath { set; get; }

        /// <summary>
        /// LabelId
        /// </summary>
        public string LabelId { set; get; }


        /// <summary>
        /// IsDeleted
        /// </summary>
        public bool IsDeleted { set; get; }

        /// <summary>
        /// Remark
        /// </summary>
        public string Remark { set; get; }

        /// <summary>
        /// TenantId
        /// </summary>
        public string TenantId { set; get; }
    }
}
