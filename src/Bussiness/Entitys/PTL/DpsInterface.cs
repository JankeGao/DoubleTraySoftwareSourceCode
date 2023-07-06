using HP.Core.Data;
using HP.Data.Orm.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Bussiness.Entitys.PTL
{
    [Table("DPSINTERFACE")]
    public class DpsInterface : EntityBase<int>
    {
        /// <summary>
        /// 获取或设置主键。
        /// </summary>
        public string OperateId { get; set; }

        /// <summary>
        /// 获取或设置所属单号。
        /// </summary>
        public string ProofId { get; set; }

        /// <summary>
        /// 获取或设置库位码。
        /// </summary>
        public string LocationId { get; set; }

        /// <summary>
        /// 获取或设置批号。
        /// </summary>
        public string BatchNO { get; set; }

        /// <summary>
        /// 获取或设置品名。
        /// </summary>
        public string GoodsName { get; set; }

        /// <summary>
        /// 获取或设置产地。
        /// </summary>
        public string MakerName { get; set; }

        /// <summary>
        /// 获取或设置规格。
        /// </summary>
        public string Spec { get; set; }

        /// <summary>
        /// 获取或设置应拣数量。
        /// </summary>
        public int ?Quantity { get; set; }

        /// <summary>
        /// 获取或设置实拣数量。
        /// </summary>
        public int? RealQuantity { get; set; }

        /// <summary>
        /// 获取或设置状态。
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 获取或设置下发时间。
        /// </summary>
        public string CreateDate { get; set; }

        /// <summary>
        /// 获取或设置周转箱号。
        /// </summary>
        public string ToteId { get; set; }

        /// <summary>
        /// 获取或设置拣货员工牌。
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// 获取或设置拣选时间。
        /// </summary>
        public string PickedDate { get; set; }
        /// <summary>
        /// 关联的ID  拣货明细  拆盘明细 或上架明细
        /// </summary>
        public int RelationId { get; set; }
        /// <summary>
        /// 物料条码
        /// </summary>
        public string MaterialLabel { get; set; }

        [NotMapped]
        public string StatusCaption
        {
            get
            {
                if (Status==0)
                {
                    return "初始状态";
                }
                else if(Status==1)
                {
                    return "亮灯中";
                }
                else
                {
                    return "已完成";
                }
            }
        }
    }
    [Description("PTL任务明细视图")]
    [Table("VIEW_DPSINTERFACE")]
    public class DpsInterfaceVM : DpsInterface
    {
        public string XGateIp { get; set; }

        public int DeviceAddress { get; set; }
        [NotMapped]
        public int BusIndex
        {
            get
            {
                if (DeviceAddress >= 0 && DeviceAddress <= 50)
                {
                    return 0;
                }
                else if (DeviceAddress > 50 && DeviceAddress <= 100)
                {
                    return 1;
                }
                else if (DeviceAddress > 100 && DeviceAddress <= 150)
                {
                    return 2;
                }
                else
                {
                    return 3;
                }
            }
        }

        public int? M3DeviceAddress { get; set; }

        public int? M3Index { get; set; }

        public int? DeviceType { get; set; }

        public string  Row { get; set; }

        public string Column { get; set; }

        public string M3XgateIp { get; set; }
        /// <summary>
        /// 巷道编码
        /// </summary>
        public int ChannelId { get; set; }

        public string SkuSubCode { get; set; }
        [NotMapped]
        public int M3BusIndex
        {
            get
            {
                if (M3DeviceAddress >= 0 && M3DeviceAddress <= 50)
                {
                    return 0;
                }
                else if (M3DeviceAddress > 50 && M3DeviceAddress <= 100)
                {
                    return 1;
                }
                else if (M3DeviceAddress > 100 && M3DeviceAddress <= 150)
                {
                    return 2;
                }
                else
                {
                    return 3;
                }
            }
        }
        /// <summary>
        ///  获取或设置订单类型。0拣货单 1 拆盘单 2 上架任务灯 3 库存点亮
        /// </summary>
        public int OrderType { get; set; }

        public string MaterialName { get; set; }
        public string OrderCode { get; set; }


        public string AreaCode { get; set; }

        public string WareHouseCode { get; set; }

        public bool IsElectronicMateria { get; set; }
        // public string AreaCode { get; set; }
    }
}