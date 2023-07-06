using HP.Core.Data;
using HP.Core.Functions;
using HP.Utility;
using Newtonsoft.Json;

namespace HPC.BaseService.Dtos
{
    public class ModuleOutputDto : IOutputDto
    {
        [JsonProperty("id")]
        public string TreeId { set; get; }
        [JsonProperty("text")]
        public string TreeText { set; get; }
        [JsonProperty("iconCls")]
        public string TreeIcon { set; get; }
        [JsonProperty("state")]
        public string TreeState { set; get; }
        public int Id { set; get; }
        public string Code { set; get; }
        public string Name { set; get; }
        public string ParentCode { set; get; }
        public int Sort { set; get; }
        public string Icon { set; get; }
        public string Address { set; get; }
        public string Remark { set; get; }
        public bool Enabled { set; get; }
        public string Target { set; get; }
        public string AuthType { set; get; }
        public string Type { set; get; }
        public string TypeCaption
        {
            get
            {
                return EnumHelper.GetCaption(typeof(ModuleType), Type);
            }
        }
        public string AuthTypeCaption
        {
            get
            {
                return EnumHelper.GetCaption(typeof(AuthTypeEnum), AuthType);
            }
        }
    }
}
