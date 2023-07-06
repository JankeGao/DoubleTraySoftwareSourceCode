using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Bussiness.Contracts;
using Bussiness.Dtos;
using HP.Core.Data;
using HP.Data.Orm.Entity;
using Bussiness.Entitys;
using HP.Data.Orm;
using HP.Utility.Data;
using HP.Core.Sequence;

namespace Bussiness.Services
{
    class BoxServer : Contracts.IBoxContract
    {
        public IRepository<Box, int> BoxRepository { get; set; }

        public IMaterialContract MaterialContract { set; get; }

        public ISequenceContract SequenceContract { set; get; }
        public IQuery<Box> Box
        {
            get
            {
                return BoxRepository.Query();
            }
        }


        /// <summary>
        /// 仓库契约
        /// </summary>
        public IWareHouseContract WareHouseContract { set; get; }

        //判断该箱的编码是否存在并创建
        public DataResult CreateBox(Box entity)
        {
            // 判断是否有入库单号
            if (String.IsNullOrEmpty(entity.Code))
            {
                entity.Code = SequenceContract.Create(entity.GetType());
            }
            if (Box.Any(a => a.Code == entity.Code))
            {
                return DataProcess.Failure(string.Format("载具箱的编码{0}已存在", entity.Code));
            }
            
            if (BoxRepository.Insert(entity))
            {
                return DataProcess.Success(string.Format("载具箱的编码{0}创建成功", entity.Code));
            }
            return DataProcess.Failure();
        }

        //删除箱的操作
        public DataResult DeleteBox(int id)
        {
            // 验证id 是否存在
            var entity = BoxRepository.GetEntity(id);
            if (entity == null)
            {
                return DataProcess.Failure(string.Format("载具型号{0}在系统中不存在", entity.Code));
            }
            // 验证在仓库储位中是否还要存放的载具
            if (WareHouseContract.Locations.Any(a => a.BoxCode == entity.Code))
            {
                return DataProcess.Failure(string.Format("载具型号{0}在仓库中仍存在对应储位使用，无法删除", entity.Code));
            }
            if (BoxRepository.LogicDelete(id) > 0)
            {
                return DataProcess.Success("删除成功");
            }
            return DataProcess.Failure();
        }

        //对箱的信息进行编辑
        public DataResult EditBox(Box entity)
        {
            if (BoxRepository.Update(entity) > 0)
            {
                return DataProcess.Success(string.Format("供应商{0}编辑成功", entity.FileID, entity.Id));
            }
            return DataProcess.Failure();
        }
    }
}
