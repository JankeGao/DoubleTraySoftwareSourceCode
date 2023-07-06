using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLCServer.Common
{
    public class ApiQuantityChange
    {
        public string AreaCode { get; set; }

        public string LocationCode { get; set; }//相对于货架库位

        public int CurrentQuantity { get; set; }//当前库存数量

        public string LocationId
        {
            get
            {
                if (!string.IsNullOrEmpty(LocationCode))
                {
                    var array = LocationCode.Split('/');
                    var row = array[0] == "A" ? "1" : "";
                    var col = int.Parse(array[1]).ToString();
                    return "40-" + AreaCode + "-" + row+"-"+col;
                }
                return "";
         
            }
        }
    }
}
