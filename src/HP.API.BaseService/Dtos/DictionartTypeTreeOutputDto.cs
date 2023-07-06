using System.Collections.Generic;

namespace HPC.BaseService.Dtos
{
   public class DictionartTypeTreeOutputDto:HPC.BaseService.Models.DictionaryType
    {
        public List<DictionartTypeTreeOutputDto> children { set; get; }

        public string State { get; set; }

        public bool Checked { get; set; }
    }
}
