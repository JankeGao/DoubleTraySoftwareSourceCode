using System;
using System.Collections.Generic;
using HP.Core.Data;
using HP.Utility;
using HPC.BaseService.Enums;
using HPC.BaseService.Models;

namespace HPC.BaseService.Dtos
{
    public class RoleOutputDto:IOutputDto
    {
        /// <summary>
        /// 编码
        /// </summary>
        public int Id { set; get; }
        /// <summary>
        /// 编码
        /// </summary>
        public string Code { set; get; }
        /// <summary>
        /// 角色名称
        /// </summary>
        public string Name { set; get; }
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Enabled { set; get; }
        /// <summary>
        /// 系统角色
        /// </summary>
        public bool IsSystem { set; get; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { set; get; }

        /// <summary>
        /// 部门的上级编码
        /// </summary>
        public string ParentCode { set; get; }

        /// <summary>
        /// 部门编码
        /// </summary>
        public string DepartmentCode { set; get; }


        /// <summary>
        /// 部门名称
        /// </summary>
        public string DepartmentName { set; get; }
    }
}