using Bussiness.Entitys;
using System;

namespace Bussiness.Dtos
{
    public class WareHouseDto : WareHouse
    {
        /// <summary>
        ///  货柜信息
        /// </summary>
        public System.Collections.Generic.List<ContainerDto> children { get; set; }

    }   
}