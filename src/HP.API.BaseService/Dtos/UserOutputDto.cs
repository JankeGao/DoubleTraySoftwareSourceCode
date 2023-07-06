using System;
using System.Collections.Generic;
using HP.Core.Data;
using HP.Utility;
using HPC.BaseService.Enums;
using HPC.BaseService.Models;

namespace HPC.BaseService.Dtos
{
    public class UserOutputDto: IOutputDto
    {
        /// <summary>
        /// 编号
        /// </summary>
        public int Id { set; get; }
        /// <summary>
        /// 编码
        /// </summary>
        public string Code { set; get; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { set; get; }

        /// <summary>
        /// token
        /// </summary>
        public string Token { set; get; }

        /// <summary>
        /// 性别
        /// </summary>
        public int Sex{ set; get; }
        public string SexCaption
        {
            get { return EnumHelper.GetCaption(typeof(SexEnum), Sex); }
        }
        /// <summary>
        /// 角色
        /// </summary>
        public List<Role> Role { set; get; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { set; get; }
        /// <summary>
        /// 移动电话
        /// </summary>
        public string Mobilephone { set; get; }
        /// <summary>
        /// WeXin
        /// </summary>                       
        public string WeXin { get; set; }
        /// <summary>
        /// 头像
        /// </summary>
        public string Header { set; get; }
        /// <summary>
        /// 启用
        /// </summary>
        public bool Enabled { set; get; }
        /// <summary>
        /// 系统帐号
        /// </summary>
        public bool IsSystem { set; get; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { set; get; }
        /// <summary>
        /// 创建人编码
        /// </summary>
        public string CreatedUserCode { set; get; }
        /// <summary>
        /// 创建人姓名
        /// </summary>
        public string CreatedUserName { set; get; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedTime {set;get;}
        /// <summary>
        /// 更新人编码
        /// </summary>
        public string UpdatedUserCode { set; get; }
        /// <summary>
        /// 更新人姓名
        /// </summary>
        public string UpdatedUserName { set; get; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? UpdatedTime { set; get; }
        /// <summary>
        /// 加入日期
        /// </summary>
        public DateTime JoinDate { set; get; }


        /// <summary>
        /// 头像
        /// </summary>
        public string PictureUrl { set; get; }


        /// <summary>
        /// 头像
        /// </summary>
        public int FileId { set; get; }




        /// <summary>
        /// 头像
        /// </summary>
        public string RFIDCode { set; get; }
    }
}