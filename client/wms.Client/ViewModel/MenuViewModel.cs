using System;
using wms.Client.Core.Interfaces;
using wms.Client.LogicCore.Common;
using wms.Client.LogicCore.Configuration;
using wms.Client.Model.Entity;
using wms.Client.Model.Query;
using wms.Client.ViewModel.Base;

namespace wms.Client.ViewModel
{
    /// <summary>
    /// 菜单
    /// </summary>
   // [Module(ModuleType.BasicData, "MenuDlg", "菜单管理")]
    public class MenuViewModel : DataProcess<Menu>
    {
        private readonly IMenuService service;
        public MenuViewModel()
        {
            service = ServiceProvider.Instance.Get<IMenuService>();
            this.Init();
        }

        public override async void GetPageData(int pageIndex)
        {
            try
            {
                var r = await service.GetMenusAsync(new MenuParameters()
                {
                    PageIndex = pageIndex,
                    PageSize = PageSize,
                    Search = SearchText,
                });
                if (r.success)
                {
                    TotalCount = r.TotalRecord;
                    GridModelList.Clear();
                    r.menus.ForEach((arg) => GridModelList.Add(arg));
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
