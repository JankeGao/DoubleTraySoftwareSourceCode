using System.Collections.Generic;
using HP.Core.Dependency;
using HP.Data.Orm;
using HP.Utility.Data;
using HPC.BaseService.Dtos;
using HPC.BaseService.Models;

namespace HPC.BaseService.Contracts
{
    public interface IDepartmentContract : IScopeDependency
    {
        IQuery<DepartmentTree> DepartmentTree { get; }

        IQuery<Department> Departments { get; }

        /// <summary>
        /// 生成部门树
        /// </summary>
        /// <param name="pageCondition"></param>
        /// <returns></returns>
        List<DepartmentTree> GetTree(PageCondition pageCondition);

        /// <summary>
        /// 创建部门
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        DataResult CreateDepartment(Department entity);

        /// <summary>
        /// 编辑部门
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        DataResult EditDepartment(Department entity);

        /// <summary>
        /// 移除部门
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        DataResult RemoveDepartment(int id);


        List<DepartmentTree> InitModuleFunctionTree(List<DepartmentTree> list, string parentCode);
    }
}