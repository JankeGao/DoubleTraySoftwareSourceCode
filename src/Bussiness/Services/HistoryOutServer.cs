using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using HP.Core.Data;
using HP.Data.Orm.Entity;
using Bussiness.Entitys;
using HP.Data.Orm;
using HP.Utility.Data;
using Bussiness.Dtos;
using HPC.BaseService.Contracts;

namespace Bussiness.Services
{
    class HistoryOutServer : Contracts.IHistoryOutContract
    {

        public Bussiness.Contracts.IOutContract OutContract { set; get; }

        public Bussiness.Contracts.IWareHouseContract WareHouseContract { set; get; }
        public Bussiness.Contracts.IMaterialContract MaterialContract { set; get; }

        public IIdentityContract IdentityContract { get; set; }

        

        public IQuery<OutMaterialLabelDto> OutMaterialLabelDtos
        {
            get
            {
                //出库信息和明细信息和材料信息和仓库
                return OutContract.OutMaterialLabels
                    .InnerJoin(WareHouseContract.LocationVMs, (outs, location) => outs.LocationCode == location.Code)
                    .InnerJoin(MaterialContract.Materials, (outs, location, material) => outs.MaterialCode == material.Code)
                    .LeftJoin(IdentityContract.Users, (outs, location, material, user) => outs.Operator == user.Code)
                    .LeftJoin(OutContract.Outs, (outs, location, material, user, outentity) => outs.OutCode == outentity.Code)
                    .Select((outs, location, material, user, outentity) => new OutMaterialLabelDto()
                    {
                        Id = outs.Id,
                        OutCode = outs.OutCode,
                        IsDeleted = outs.IsDeleted,
                        Quantity = outs.Quantity,
                        MaterialLabel= outs.MaterialLabel,
                        MaterialUnit =  material.Unit,
                        Remark = outentity.Remark,
                        Remark2 = material.Remark2,
                        MaterialCode = material.Code,
                        MaterialName = material.Name,
                        WareHouseCode = location.WareHouseCode,
                        WareHouseName = location.WarehouseName,
                        ContainerCode = location.ContainerCode,
                        TrayCode = location.TrayCode,
                        LocationCode = location.Code,
                        CheckedTime= outs.CheckedTime,
                        BoxUrl = location.BoxUrl,
                        BoxName = location.BoxName,
                        CreatedUserName = outs.CreatedUserName,
                        CreatedTime = outs.CreatedTime,
                        PickedTime = outs.PickedTime,
                        Operator = outs.Operator,
                        OperatorName= user.Name
                    });
            }
        }
    }
}
