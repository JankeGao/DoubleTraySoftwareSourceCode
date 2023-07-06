using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bussiness.Dtos
{
    public class TrayUserDto : Entitys.TrayUserMap
    {

        /// <summary>
        /// 社保品牌
        /// </summary>
        public string UserName { get; set; }
        public string Role { get; set; }


    }
}
