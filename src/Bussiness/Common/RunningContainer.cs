using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bussiness.Common
{
    public class RunningContainer
    {
        /// <summary>
        /// 货架编码
        /// </summary>
        public string ContainerCode { get; set; }
        /// <summary>
        /// 托盘序号
        /// </summary>
        public int TrayCode { get; set; }
        /// <summary>
        /// X轴灯号
        /// </summary>
        public int XLight{ get; set; }
        /// <summary>
        /// X轴亮灯长度
        /// </summary>
        public int XLenght { get; set; }

        public string IpAddress { get; set; }

        public int Port { get; set; }

        public int ContainerType { get; set; }

        public int BracketNumber { get; set; }
    }
}
