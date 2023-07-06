using System.Threading.Tasks;
using wms.Client.Model.Query;
using wms.Client.Model.ResponseModel;

namespace wms.Client.Core.Interfaces
{
    public interface IDictionariesService
    {
        Task<DictionariesResponse> GetDictionariesAsync(DictionariesParameters parameters);

    }
}
