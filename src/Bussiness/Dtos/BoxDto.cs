using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bussiness.Dtos
{
    public class BoxDto : Entitys.Box
    {
        public string MaterialName { get; set; }
        public string MaterialCode { get; set; }
    }
}
