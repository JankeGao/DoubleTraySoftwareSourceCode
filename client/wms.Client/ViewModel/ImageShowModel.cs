using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using HP.Core.Dependency;
using HP.Core.Mapping;
using HP.Core.Security;
using HP.Utility;
using HPC.BaseService.Contracts;
using HPC.BaseService.Dtos;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using wms.Client.Core.Interfaces;
using wms.Client.LogicCore.Configuration;
using wms.Client.LogicCore.Helpers;
using wms.Client.LogicCore.Helpers.Files;
using wms.Client.Model.ResponseModel;
using wms.Client.Service;
using wms.Client.ViewModel;

namespace wms.Client.ViewModel
{
    /// <summary>
    /// 操作人员登录
    /// </summary>
    public class ImageShowModel : ViewModelBase
    {







        #region 命令(Binding Command)

        private RelayCommand _userLoginCommand;
        private RelayCommand _exitCommand;

        public RelayCommand ExitCommand
        {
            get
            {
                if (_exitCommand == null)
                {
                    _exitCommand = new RelayCommand(() => ApplicationShutdown());
                }
                return _exitCommand;
            }
        }

        #endregion


        /// <summary>
        /// 关闭系统
        /// </summary>
        public void ApplicationShutdown()
        {
            Messenger.Default.Send("", "ApplicationShutdown");
        }


    }
}
