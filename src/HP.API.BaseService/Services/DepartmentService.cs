using System.Collections.Generic;
using System.Linq;
using HP.Core.Data;
using HP.Core.Mapping;
using HP.Data.Entity.Pagination;
using HP.Data.Orm;
using HP.Utility.Data;
using HP.Utility.Extensions;
using HPC.BaseService.Contracts;
using HPC.BaseService.Dtos;
using HPC.BaseService.Models;

namespace HPC.BaseService.Services
{
    public class DepartmentService:IDepartmentContract
    {
        public IRepository<Department,int> DepartmentRepository { set; get; }

        public IIdentityContract IdentityContract { set; get; }
        
        public IMapper Mapper { set; get; }

        public IQuery<Department> Departments
        {
            get { return DepartmentRepository.Query(); }
        }

        public IQuery<DepartmentTree> DepartmentTree
        {
            get
            {
                return Departments
                    .Select((department) => new DepartmentTree
                    {
                        TreeId = department.Code,
                        TreeText = department.Name,
                        Id = department.Id,
                        Code = department.Code,
                        Name = department.Name,
                        ShortName = department.ShortName,
                        NatureDict = department.NatureDict,
                        Telephone = department.Telephone,
                        ENumber = department.ENumber,
                        ParentCode = department.ParentCode,
                        Flag = department.Flag,
                        Remark = department.Remark,
                        CreatedUserCode = department.CreatedUserCode,
                        CreatedUserName = department.CreatedUserName,
                        CreatedTime = department.CreatedTime,
                    });
            }
        }

        /// <summary>
        /// 生成部门树
        /// </summary>
        /// <param name="pageCondition"></param>
        /// <returns></returns>
        public List<DepartmentTree> GetTree(PageCondition pageCondition)
        {
            var query = DepartmentTree;

            if (pageCondition.FilterRuleCondition.Count > 0)
            {
                var filter = pageCondition.FilterRuleCondition.Find(a => a.Field == "Code");
                if (filter != null)
                {
                    string flag = "/{0}/".FormatWith(filter.Value);
                    query = query.Where(a => a.Code.Contains(flag));
                    pageCondition.FilterRuleCondition.Remove(filter);
                }
            }

            var departments = query.Where(pageCondition).ToList();

            return InitTree(departments, null);
        }

        /// <summary>
        /// 生成部门树
        /// </summary>
        /// <param name="list"></param>
        /// <param name="parentCode"></param>
        /// <returns></returns>
        public List<DepartmentTree> InitTree(List<DepartmentTree> list, string parentCode)
        {
            List<DepartmentTree> trees = new List<DepartmentTree>();
            foreach (var department in list.Where(a => a.ParentCode == parentCode))
            {
                DepartmentTree tree = Mapper.MapTo<DepartmentTree>(department);
                tree.State = TreeStateEnum.Open.ToString().ToLower();
                tree.TreeId = department.Code;
                tree.TreeText = department.Name;
                tree.Children = InitTree(list, department.Code);
                trees.Add(tree);
            }

            return trees;
        }

        /// <summary>
        /// 初始化模块功能树
        /// </summary>
        /// <param name="modules">权限模块</param>
        /// <param name="parentCode">上级模块编码</param>
        /// <returns></returns>
        public List<DepartmentTree> InitModuleFunctionTree(List<DepartmentTree> list, string parentCode)
        {
            List<DepartmentTree> trees = new List<DepartmentTree>();

            List<DepartmentTree> tmpModules = null;
            if (parentCode.IsNullOrEmpty())
            {
                tmpModules = list.FindAll(a => string.IsNullOrEmpty(a.ParentCode));
            }
            else
            {
                tmpModules = list.FindAll(a => a.ParentCode == parentCode);
            }

            foreach (var tree in tmpModules)
            {

                if (list.Exists(a => a.ParentCode == tree.Code))
                {
                    tree.State = TreeStateEnum.Closed.ToString().ToLower();
                    tree.Children = InitModuleFunctionTree(list, tree.Code);
                }
                else
                {
                    tree.State = TreeStateEnum.Open.ToString().ToLower();
                }

                trees.Add(tree);
            }

            return trees;
        }

        /// <summary>
        /// 创建部门
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public DataResult CreateDepartment(Department entity)
        {
            DataResult result = ValidateDepartment(entity);
            if (!result.Success) return result;

            if (entity.Code.IsNullOrEmpty())
            {
                return DataProcess.Failure("部门编码不能为空！");
            }

            //验证部门编码是否存在
            if (Departments.Any(a => a.Code == entity.Code))
            {
                return DataProcess.Failure("部门编码({0})已经存在！".FormatWith(entity.Code));
            }

            //插入部门
            if (!DepartmentRepository.Insert(entity))
            {
                return DataProcess.Failure("部门({0})创建失败！".FormatWith(entity.Code));
            }

            return DataProcess.Success("部门({0})创建成功！".FormatWith(entity.Code));
        }

        /// <summary>
        /// 编辑部门
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public DataResult EditDepartment(Department entity)
        {
            DataResult result = ValidateDepartment(entity);
            if (!result.Success) return result;

            entity.Id.CheckGreaterThan("Id", 0);

            //获取原始部门数据
            var oriEntity = Departments.Where(a => a.Id == entity.Id).Select(a => new {a.Flag, a.Code, a.ParentCode})
                .FirstOrDefault();
            oriEntity.CheckNotNull("oriEntity");

            entity.Code = oriEntity.Code;
            //生成标示
            entity.Flag = oriEntity.Flag;
            if (entity.ParentCode.IsNullOrEmpty())
            {
                entity.Flag = "/{0}/".FormatWith(oriEntity.Code);
            }
            else
            {
                if (oriEntity.ParentCode != entity.ParentCode)
                {
                    var parentOriEntity = Departments.Where(a => a.Code == entity.ParentCode)
                        .Select(a => new {a.Flag}).FirstOrDefault();
                    entity.Flag = parentOriEntity.Flag + oriEntity.Code + "/";
                }
            }

            DepartmentRepository.UnitOfWork.TransactionEnabled = true;

            if (DepartmentRepository.Update(a => new Department
            {
                Name = entity.Name,
                ShortName = entity.ShortName,
                NatureDict = entity.NatureDict,
                Telephone = entity.Telephone,
                ENumber = entity.ENumber,
                ParentCode = entity.ParentCode,
                OrganizationCode = entity.OrganizationCode,
                Manager = entity.Manager,
                Flag = entity.Flag,
                Remark = entity.Remark
            }, a => a.Id == entity.Id) == 0)
            {
                return DataProcess.Failure("部门({0})编辑失败！".FormatWith(oriEntity.Code));
            }

            //更新所有子部门标示
            if (Departments.Any(a => a.ParentCode == oriEntity.Code))
            {
                if (DepartmentRepository.Update(
                    a => new Department { Flag = entity.Flag + a.Flag.Substring(oriEntity.Flag.Length) },
                    a => a.Flag.Contains(oriEntity.Flag) && a.Flag != oriEntity.Flag) == 0)
                {
                    return DataProcess.Failure("子部门标示更新失败！");
                }
            }

            DepartmentRepository.UnitOfWork.Commit();

            return DataProcess.Success("部门({0})编辑成功！".FormatWith(oriEntity.Code));
        }

        /// <summary>
        /// 移除部门
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DataResult RemoveDepartment(int id)
        {
            id.CheckGreaterThan("id", 0);

            var oriEntity = Departments.Where(a => a.Id == id).Select(a => new {a.ParentCode, a.Code}).FirstOrDefault();
            oriEntity.CheckNotNull("oriEntity");

            if (Departments.Any(a => a.ParentCode == oriEntity.Code))
            {
                return DataProcess.Failure("部门({0})存在子部门！".FormatWith(oriEntity.Code));
            }

            if (IdentityContract.RoleDtos.Any(a => a.DepartmentCode == oriEntity.Code))
            {
                return DataProcess.Failure("部门({0})存在用户组，请先删除！".FormatWith(oriEntity.Code));
            }

            if (DepartmentRepository.Delete(id) == 0)
            {
                return DataProcess.Failure("部门({0})移除失败".FormatWith(oriEntity.Code));
            }

            return DataProcess.Success("部门({0})移除成功！".FormatWith(oriEntity.Code));
        }

        /// <summary>
        /// 验证数据合法性
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private DataResult ValidateDepartment(Department entity)
        {
            if (entity.Name.IsNullOrEmpty())
            {
                return DataProcess.Failure("部门名称不能为空！");
            }

            //if (entity.OrganizationCode.IsNullOrEmpty())
            //{
            //    return DataProcess.Failure("机构不能为空！");
            //}

            //验证组织机构是否存在
            //if (!entity.OrganizationCode.IsNullOrEmpty())
            //{
            //    if (!OrganizationContract.Organizations.Any(a => a.Code == entity.OrganizationCode))
            //    {
            //        return DataProcess.Failure("部门所属组织机构({0})不存在！".FormatWith(entity.OrganizationCode));
            //    }
            //}

            return DataProcess.Success();
        }
    }
}
