using System;
using System.Collections.Generic;
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
    /// 数据字典
    /// </summary>
 //   [Module(ModuleType.BasicData, "DictionaryDlg", "字典管理")]
    public class DictionaryViewModel : DataProcess<Dictionaries>
    {
        private readonly IDictionariesService service;

        public DictionaryViewModel()
        {
            service = ServiceProvider.Instance.Get<IDictionariesService>();
            this.Init();
        }

        public List<DictionaryType> TypeList { get; set; }

        public override void LoadModuleAuth()
        {
            var customs = this.GetType().GetCustomAttributes(typeof(ModuleAttribute), false);
            if (customs.Length > 0)
            {
                var attr = (ModuleAttribute)customs[0];
                //访问缓存权限,设置AuthValue
            }
            base.LoadModuleAuth();
        }

        public override async void GetPageData(int pageIndex)
        {
            try
            {
                var r = await service.GetDictionariesAsync(new DictionariesParameters()
                {
                    PageIndex = pageIndex,
                    PageSize = PageSize,
                    Search = SearchText,
                });
                if (r.success)
                {
                    TotalCount = r.TotalRecord;
                    GridModelList.Clear();
                    r.Dictionaries.ForEach((arg) => GridModelList.Add(arg));
                    base.SetPageCount();
                }
            }
            catch (Exception ex)
            {
                Msg.Error(ex.Message);
            }
        }
        
        public override async void Del<TModel>(TModel model)
        {

        }

        public override async void Save()
        {

        }
    }
}
