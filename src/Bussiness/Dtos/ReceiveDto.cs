using Bussiness.Entitys;
using System;

namespace Bussiness.Dtos
{
    public class ReceiveDto : Receive
    {
        /// <summary>
        /// 仓库名称
        /// </summary>
        public string WarehouseName { get; set; }

        /// <summary>
        /// 领用用户名称
        /// </summary>
        public string ReceiveUserName { get; set; }

    }   
}