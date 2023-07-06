using Bussiness.Entitys;
using System;

namespace Bussiness.Dtos
{
    public class ReceiveTaskDto : ReceiveTask
    {
        /// <summary>
        /// 货柜储位
        /// </summary>
        public string LocationCode { get; set; }

        /// <summary>
        /// 领用用户名
        /// </summary>
        public string ReceiveUserName { get; set; }

    }
}