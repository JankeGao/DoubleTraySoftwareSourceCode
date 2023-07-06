namespace Bussiness.Dtos
{
    public class PickDto : Entitys.SMT.WmsPickMain
    {
        public string WareHouseName { get; set; }
        public string PickDictDescription { get; set; }

        public override string StatusCaption
        {
            get
            {
                if (Status != null)
                {
                    return HP.Utility.EnumHelper.GetCaption(typeof(Bussiness.Enums.SMT.PickStatusEnum), Status.GetValueOrDefault());
                }
                return "";
            }
        }
    }
}