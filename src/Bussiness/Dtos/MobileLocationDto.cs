using Bussiness.Entitys;

namespace Bussiness.Dtos
{
    public class MobileLocationDto : MobileLocation
    {
        public override string StatusCaption {
            get
            {
                if (Status != null)
                {
                    return HP.Utility.EnumHelper.GetCaption(typeof(Bussiness.Enums.MobileLocationStatusEnum), Status.Value);
                }
                return "";
            }
        }
    }
}