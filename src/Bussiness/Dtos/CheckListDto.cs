using Bussiness.Entitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bussiness.Dtos
{
    /// <summary>
    /// 盘点单Dto
    /// </summary>
    public class CheckListDto : CheckList
    {

        /// <summary>
        /// 仓库名称
        /// </summary>
        public string WareHouseName { get; set; }
    }
}
