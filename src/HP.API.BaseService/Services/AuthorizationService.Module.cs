using System;
using System.Collections.Generic;
using HP.Core.Functions;
using HP.Core.Security;
using HP.Data.Orm;
using HP.Utility;
using HP.Utility.Data;
using HP.Utility.Extensions;
using HPC.BaseService.Dtos;
using HPC.BaseService.Models;

namespace HPC.BaseService.Services
{
    public partial class AuthorizationService
    {

        public IQuery<Models.Module> Modules
        {
            get { return ModuleRepository.Query(); }
        }

        /// <summary>
        /// 创建模块
        /// </summary>
        /// <param name="entity">entity</param>
        /// <returns></returns>
        public DataResult CreateModule(Models.Module entity)
        {
            var result = ValidateModule(entity);
            if (!result.Success) return result;

            if (entity.Code.IsNullOrEmpty())
            {
                return DataProcess.Failure("请输入模块编码！");
            }
            entity.AuthType = "Authorization";
            //验证菜单编码是否存在
            if (Modules.Any(p => p.Code == entity.Code))
            {
                return DataProcess.Failure("模块编码({0})已经存在！".FormatWith(entity.Code));
            }
            ModuleRepository.UnitOfWork.TransactionEnabled = true;

            if (!ModuleRepository.Insert(entity))
            {
                return DataProcess.Failure("模块({0})创建成功！".FormatWith(entity.Code));
            }

            // 为系统用户自动授权
            if (!ModuleAuthRepository.Insert(new ModuleAuth()
            {
                Type = 1,
                TypeCode = "admin",
                ModuleCode = entity.Code
            }))
            {
                return DataProcess.Failure("系统管理员授权失败");
            }

            ModuleRepository.UnitOfWork.Commit();

            return DataProcess.Success("模块({0})创建成功！".FormatWith(entity.Code));
        }

        /// <summary>
        /// 编辑模块
        /// </summary>
        /// <param name="entity">entity</param>
        /// <returns></returns>
        public DataResult EditModule(Models.Module entity)
        {
            entity.Id.CheckGreaterThan("Id", 0);

            DataResult validate = ValidateModule(entity);
            if (!validate.Success) return validate;

            //获取原始菜单信息
            var oriEntity = Modules.Where(a => a.Id == entity.Id).Select(a => new { a.Code }).First();
            entity.AuthType = "Authorization";
            if (ModuleRepository.Update(a => new Models.Module()
            {
                Name = entity.Name,
                ParentCode = entity.ParentCode,
                Address = entity.Address,
                Icon = entity.Icon,
                Sort = entity.Sort,
                Remark = entity.Remark,
                Enabled = entity.Enabled,
                Target = entity.Target,
                AuthType = entity.AuthType,
                Type = entity.Type
            }, a => a.Id == entity.Id) == 0)
            {
                return DataProcess.Failure("模块({0})编辑失败！".FormatWith(oriEntity.Code));
            }

            return DataProcess.Success("模块({0})编辑成功！".FormatWith(oriEntity.Code));
        }

        /// <summary>
        /// 移除模块
        /// </summary>
        /// <param name="id">模块主键</param>
        /// <returns></returns>
        public DataResult RemoveModule(int id)
        {
            id.CheckGreaterThan("id", 0);

            var oriEntity = Modules.Where(a => a.Id == id).Select(a => new { a.Code }).First();

            ModuleRepository.UnitOfWork.TransactionEnabled = true;

            //模块移除
            if (ModuleRepository.Delete(id) == 0)
            {
                return DataProcess.Failure("模块({0})移除失败！".FormatWith(oriEntity.Code));
            }

            //模块授权移除
            if (ModuleAuths.Any(a => a.ModuleCode == oriEntity.Code))
            {
                if (ModuleAuthRepository.Delete(a => a.ModuleCode == oriEntity.Code) == 0)
                {
                    return DataProcess.Failure("模块({0})移除授权失败！".FormatWith(oriEntity.Code));
                }
            }
            // 移除子页面授权
            if (Modules.Any(a => a.ParentCode == oriEntity.Code))
            {
                var list = Modules.Where(a => a.ParentCode == oriEntity.Code).ToList();
                foreach (var item in list)
                {
                    if (ModuleAuthRepository.Delete(a => a.ModuleCode == item.Code) == 0)
                    {
                        return DataProcess.Failure("子模块({0})移除授权失败！".FormatWith(oriEntity.Code));
                    }
                }
            }
            // 移除子页面
            if (Modules.Any(a => a.ParentCode == oriEntity.Code))
            {
                var list = Modules.Where(a => a.ParentCode == oriEntity.Code ).ToList();
                foreach (var item in list)
                {
                    if (ModuleRepository.Delete(item.Id) == 0)
                    {
                        return DataProcess.Failure("子模块({0})移除失败！".FormatWith(oriEntity.Code));
                    }
                }
            }

            ModuleRepository.UnitOfWork.Commit();

            return DataProcess.Success("模块({0})信息移除成功！".FormatWith(oriEntity.Code));
        }

        /// <summary>
        /// 初始化模块功能树
        /// </summary>
        /// <param name="modules">权限模块</param>
        /// <param name="parentCode">上级模块编码</param>
        /// <returns></returns>
        public List<ModuleTreeOutputDto> InitModuleFunctionTree(List<Models.Module> modules, string parentCode)
        {
            List<ModuleTreeOutputDto> result = new List<ModuleTreeOutputDto>();

            List<Models.Module> tmpModules = null;
            if (parentCode.IsNullOrEmpty())
            {
                tmpModules = modules.FindAll(a => string.IsNullOrEmpty(a.ParentCode));
            }
            else
            {
                tmpModules = modules.FindAll(a => a.ParentCode == parentCode);
            }

            foreach (var module in tmpModules)
            {
                ModuleTreeOutputDto tree = new ModuleTreeOutputDto()
                {
                    Id = module.Id,
                    Code = module.Code,
                    Name = module.Name,
                    ParentCode = module.ParentCode,
                    Address = module.Address,
                    Target = module.Target,
                    Icon = module.Icon,
                    Type = module.Type,
                    Sort = module.Sort,
                    Enabled = module.Enabled

                };

                if (modules.Exists(a => a.ParentCode == module.Code))
                {
                    tree.State = TreeStateEnum.Closed.ToString().ToLower();
                    tree.children = InitModuleFunctionTree(modules, module.Code);
                }
                else
                {
                    tree.State = TreeStateEnum.Open.ToString().ToLower();
                    tree.Checked = module.Enabled;
                }

                result.Add(tree);
            }

            return result;
        }

        /// <summary>
        /// 数据验证
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        private DataResult ValidateModule(Models.Module entity)
        {
            entity.CheckNotNull("entity");

            if (entity.Name.IsNullOrEmpty())
            {
                return DataProcess.Failure("请输入模块名称！");
            }
            if (entity.Target.IsNullOrEmpty())
            {
                return DataProcess.Failure("请选择目标！");
            }
            if (!EnumHelper.GetEnums(typeof(ModuleTarget)).Exists(a => a.Key == entity.Target))
            {
                return DataProcess.Failure("目标({0})数据异常！");
            }

            return DataProcess.Success();
        }
    }
}
