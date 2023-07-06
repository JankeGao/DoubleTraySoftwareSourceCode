using System.Threading.Tasks;
using Bussiness.Dtos;
using Bussiness.Entitys;
using HP.Utility.Data;

namespace wms.Client.Core.Interfaces
{
    public interface ILabelService
    {
        Task<DataResult> PostCreateLabel(Label model);


    }
}
