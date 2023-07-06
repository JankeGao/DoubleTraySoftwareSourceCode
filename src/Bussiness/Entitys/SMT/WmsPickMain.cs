using HP.Core.Data;
using HP.Data.Orm.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace  Bussiness.Entitys.SMT
{
    /// <summary>
    /// 初始单据表
    /// </summary>
    [Table("TB_WMS_PICK_MAIN")]
    public class WmsPickMain : ServiceEntityBase<int>
    {
        /// <summary>
        /// 账本ID
        /// </summary>
        public int? Org_Id { get; set; }
        /// <summary>
        /// 单据号
        /// </summary>
        public string Issue_No { get; set; }
        /// <summary>
        /// 组件料号(物料编码)
        /// </summary>
        public string Parts_No { get; set; }
        /// <summary>
        /// 部门
        /// </summary>
        public string Department_No { get; set; }
        /// <summary>
        /// 工单号
        /// </summary>
        public string Wo_No { get; set; }
        /// <summary>
        /// 计划时间
        /// </summary>
        public string  Plan_Date { get; set; }
        /// <summary>
        /// 客户编码
        /// </summary>
        public string Custom_No { get; set; }
        /// <summary>
        /// 工号
        /// </summary>
        public string User_Id { get; set; }
        /// <summary>
        ///姓名
        /// </summary>
        public string User_Name { get; set; }
        /// <summary>
        ///单据类型
        /// </summary>
        public string Issue_Type { get; set; }
        /// <summary>
        ///交易类型
        /// </summary>
        public string Trans_Type { get; set; }
        /// <summary>
        /// 拆盘单号
        /// </summary>
      //  public string SplitNo { get; set; }
        /// <summary>
        /// 标志位 0初始 1表示Wms已获取
        /// </summary>
        public int? Status { get; set; }

        [NotMapped]
        public virtual string StatusCaption
        {
            get {
                if (Status != null)
                {
                    return HP.Utility.EnumHelper.GetCaption(typeof(Bussiness.Enums.SMT.PickStatusEnum), Status.GetValueOrDefault(0));
                }
                return "";
            }
        }
        /// <summary>
        /// WMS获取时间
        /// </summary>
        public string Wms_Get_Date { get; set; }
        /// <summary>
        /// 仓库编码
        /// </summary>
        public string WareHouseCode { get; set; }
        /// <summary>
        /// PTL任务ID
        /// </summary>
        public string ProofId { get; set; }
        /// <summary>
        /// 是否工单类型(工单领料,工单补料)//带站位表
        /// </summary>
        public bool? IsIssue { get; set; }
        /// <summary>
        /// 是否允许合并
        /// </summary>
        public bool? IsCanCombine { get; set; }

        [NotMapped]
        public List<Bussiness.Entitys.SMT.WmsPickDetail> AddMaterial { get; set; }

        public string Remark { get; set; }

        public string BillCode { get; set; }
        /// <summary>
        /// 入库仓库
        /// </summary>
        public string InWareHouseCode { get; set; }
        /// <summary>
        /// 0 出库单 1 调拨单
        /// </summary>
        public int OrderType { get; set; }
    }

    [Table("VM_TB_WMS_PICK_MAIN")]
    public class WmsPickMainVM : WmsPickMain
    {
        public string OutDictDescription { get; set; }
    }
}