using System;
using HP.Core.Data;
using HP.Utility;
using HPC.BaseService.Enums;

namespace HPC.BaseService.Dtos
{
    public class UserInputDto: IInputDto<int>
    {
        /// <summary>
        /// id
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
        /// 性别
        /// </summary>
        public int Sex{ set; get; }
        public string SexCaption
        {
            get { return EnumHelper.GetCaption(typeof(SexEnum), Sex); }
        }
        /// <summary>
        /// Password
        /// </summary>                       
        public string Password { get; set; }
        /// <summary>
        /// 角色
        /// </summary>
        public string Roles { set; get; }
        /// <summary>
        /// 移动电话
        /// </summary>
        public string Mobilephone { set; get; }
        /// <summary>
        /// WeXin
        /// </summary>                       
        public string WeXin { get; set; }

        /// <summary>
        /// WeXin
        /// </summary>                       
        public string WeChatOpenId { get; set; }
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
        /// 加入日期
        /// </summary>
        public DateTime JoinDate { set; get; }

        /// <summary>
        /// 租户
        /// </summary>
        public string TenantId { set; get; }


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