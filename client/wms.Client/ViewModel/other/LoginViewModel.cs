

using DC.BaseService.Dtos;
using DC.BaseService.Interface;

namespace Consumption.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using Consumption.ViewModel.Interfaces;
    using Consumption.Shared.Common;
    using Consumption.Shared.Dto;
    using Newtonsoft.Json;
    using Microsoft.Toolkit.Mvvm.Input;
    using Microsoft.Toolkit.Mvvm.Messaging;
    using Org.BouncyCastle.Crypto.Engines;
    using DC.Utility.Data;

    /// <summary>
    /// 登录模块
    /// </summary>
    public class LoginViewModel : BaseDialogViewModel, IBaseDialog
    {
        public LoginViewModel()
        {
           this.repository = NetCoreProvider.Get<IUserServices>();
          //  this.repository = NetCoreProvider.Get<IUserRepository>();
            LoginCommand = new RelayCommand(Login); // 登录指令
        }

        #region Property
        private string userName;
        private string passWord;
        private string report;
        private string isCancel;
        private readonly IUserServices repository;

       // private readonly IIdentityContract repository;

        public string UserName
        {
            get { return userName; }
            set { userName = value; OnPropertyChanged(); }
        }

        public string PassWord
        {
            get { return passWord; }
            set { passWord = value; OnPropertyChanged(); }
        }

        public string Report
        {
            get { return report; }
            set { report = value; OnPropertyChanged(); }
        }

        public string IsCancel
        {
            get { return isCancel; }
            set { isCancel = value; OnPropertyChanged(); }
        }
        #endregion

        #region Command

        public RelayCommand LoginCommand { get; private set; }

        /// <summary>
        /// 登录系统
        /// </summary>
        private async void Login()
        {
            try
            {
                if (DialogIsOpen) return;
                if (string.IsNullOrWhiteSpace(UserName) || string.IsNullOrWhiteSpace(PassWord))
                {
                    SnackBar("请输入用户名密码!");
                    return;
                }
                DialogIsOpen = true;
                await Task.Delay(300);

                // 系统登录
                var inputDto= new LoginInfo() {Code = UserName, Password = PassWord };

                DataResult loginResult= await repository.Login(inputDto);

              //  var loginResult = await repository.LoginAsync(UserName, PassWord);

                if (!loginResult.Success )
                {
                    SnackBar(loginResult.Message);
                    return;
                }
                //var authResult = await repository.GetAuthListAsync();
                //if (authResult.StatusCode != 200)
                //{
                //    SnackBar(authResult.Message);
                //    return;
                //}

                var userDto = JsonConvert.DeserializeObject<UserInfoDto>(loginResult.Data.ToString());

                #region 关联用户信息/缓存

                Contract.Account = userDto.User.Account;
                Contract.UserName = userDto.User.UserName;
                Contract.IsAdmin = userDto.User.FlagAdmin == 1;
                Contract.Menus = userDto.Menus; //用户包含的权限信息
              //  Contract.AuthItems = JsonConvert.DeserializeObject<List<AuthItem>>(authResult.Result.ToString());

                #endregion
                //这行代码会发射到首页,Center中会定义所有的Messenger
                WeakReferenceMessenger.Default.Send(string.Empty, "NavigationPage");
            }
            catch (Exception ex)
            {
                SnackBar(ex.Message);
            }
            finally
            {
                DialogIsOpen = false;
            }
        }

        #endregion

        public override void Exit()
        {
            WeakReferenceMessenger.Default.Send(string.Empty, "Exit");
        }
    }
}
