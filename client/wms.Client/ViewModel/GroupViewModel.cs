using System;
using wms.Client.Core.Interfaces;
using wms.Client.LogicCore.Common;
using wms.Client.LogicCore.Configuration;
using wms.Client.LogicCore.Enums;
using wms.Client.Model.Entity;
using wms.Client.Model.Query;
using wms.Client.ViewModel.Base;

namespace wms.Client.ViewModel
{
    /// <summary>
    /// 组列表
    /// </summary>
  //  [Module(ModuleType.BasicData, "GroupDlg", "权限管理")]
    public class GroupViewModel : DataProcess<Group>
    {
        private readonly IGroupService service;
        public GroupViewModel()
        {
            service = ServiceProvider.Instance.Get<IGroupService>();
            this.Init();
        }

        public override async void GetPageData(int pageIndex)
        {
            try
            {
                var r = await service.GetGroupsAsync(new GroupParameters()
                {
                    PageIndex = pageIndex,
                    PageSize = PageSize,
                    Search = SearchText,
                });
                if (r.success)
                {
                    TotalCount = r.TotalRecord;
                    GridModelList.Clear();
                    r.groups.ForEach((arg) => GridModelList.Add(arg));
                    base.SetPageCount();
                }
            }
            catch (Exception ex)
            {
                Msg.Error(ex.Message);
            }
        }
    }
}
