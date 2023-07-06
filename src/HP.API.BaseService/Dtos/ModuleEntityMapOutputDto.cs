using HP.Core.Data;
using Newtonsoft.Json;

namespace HPC.BaseService.Dtos
{
    public class ModuleEntityMapOutputDto:IOutputDto
    {
        public string Id { set; get; }
        public string Name { set; get; }
        public string ModuleId { set; get; }
        [JsonProperty("checked")]
        public bool Checked { set; get; }
    }
}
