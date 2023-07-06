using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace wms.Client.ViewModel
{
    public class MaintenancePopoverViewModel : ViewModelBase
    {
        // 记录上一次彻底消除的时间
        private DateTime _lastDismissTime = DateTime.MinValue;
        // 定义验证码
        private const string VerificationCode = "666666";

        private string verification = string.Empty;
        public string Verification
        {
            get { return verification; }
            set { verification = value; RaisePropertyChanged(); }
        }


        // 用户点击了确定按钮
        public void Btn_OK_Clicked()
        {

            // 处理用户输入的验证码
            if (Verification == VerificationCode)
            {
                // 彻底消除
                _lastDismissTime = DateTime.Now;
            }
            else
            {
                MessageBox.Show("输入的验证码不正确！", "提示信息", MessageBoxButton.OK);
            }
        }
    }
}
