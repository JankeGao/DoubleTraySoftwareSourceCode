using System;
using HP.Core.Data;
using HP.Core.Logging;
using HP.Utility;

namespace HPC.BaseService.Dtos
{
    public class LogsOutputDto: IOutputDto
    {
        /// <summary>
        /// 编号
        /// </summary>
        public int Id { set; get; }
        /// <summary>
        /// 模块名称
        /// </summary>
        public string ModuleName { set; get; }

        /// <summary>
        /// 操作名称
        /// </summary>
        public string Name { set; get; }

        /// <summary>
        /// 消息
        /// </summary>
        public string Message { set; get; }

        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Success { set; get; }

        /// <summary>
        /// 消息页面地址
        /// </summary>
        public string Url { set; get; }

        /// <summary>
        /// IP地址
        /// </summary>
        public string Ip { set; get; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime BeginTime { set; get; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndTime { set; get; }

        /// <summary>
        /// 总执行时间
        /// </summary>
        public double TotalMilliseconds { set; get; }

        /// <summary>
        /// Post提交数据
        /// </summary>
        public string PostData { set; get; }
        /// <summary>
        /// 类型
        /// </summary>
        public int Type { set; get; }
        public string LogTypeCaption
        {
            get { return EnumHelper.GetCaption(typeof(LogType), Type); }
        }
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

    }
}