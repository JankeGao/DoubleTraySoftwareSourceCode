using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bussiness.Dtos
{
   public class DictionartTypeTreeOutputDto:HPC.BaseService.Models.DictionaryType
    {
        public List<DictionartTypeTreeOutputDto> children { set; get; }

        public string State { get; set; }

        public bool Checked { get; set; }
    }
}
