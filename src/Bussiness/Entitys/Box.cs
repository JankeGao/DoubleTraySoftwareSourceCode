using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HP.Core.Data;
using HP.Data.Orm.Entity;


namespace Bussiness.Entitys
{
    [Description("货箱管理")]
    [Table("TB_WMS_Box")]

    public class Box : ServiceEntityBase<int>
    {
        /// <summary>
        /// 箱的编号
        /// </summary>
        public string Code { get; set; }

        //箱的名称
        public string Name { get; set; }

        //箱的种类
        public string Type { get; set; }

        //箱的介绍
        public string IntroduceBox { get; set; }

        //箱的宽
        public int BoxWidth { get; set; }

        //箱的高
        public int BoxLength { get; set; }

        //箱的状态
        public bool? IsVirtual { get; set; }

        //删除
        public bool IsDeleted { get; set; }

        //图片路径
        public string PictureUrl { get; set; }

        //颜色
        public string BoxColour { get; set; }

        //图片id
        public int FileID { get; set; }

    }
}
