using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Interactivity;
using wms.Client.ViewModel;


namespace wms.Client.Template
{

    /// <summary>
    /// MaintenancePopover.xaml 的交互逻辑
    /// </summary>
    public partial class MaintenancePopover : Window
    {
        // 定义验证码
        private const string VerificationCode = "1234";

        public event EventHandler<MessageBoxEventArgs> Result;

        public string VerificationCodees
        {
            get { return Verificationes.Text; }
            set { Verificationes.Text = value; }
        }

        public string Context
        {
            get { return TB_Context.Text; }
            set { TB_Context.Text = value; }
        }
        

        public MaintenancePopover()
        {
            InitializeComponent();
        }
        
        // 用户点击了确定按钮
        public void Btn_OK_Click()
        {
            Close();
        }


        public static void Show(string context, EventHandler<MessageBoxEventArgs> result)
        {
            var mb = new MaintenancePopover();
            mb.Context = context;
            mb.Result += result;
            mb.Show();
        }

        public static bool ShowDialoges()
        {
            var mb = new MaintenancePopover();
            mb.ShowDialog();

            // 获取用户输入的验证码值
            var verificationCode = mb.Verificationes.Text;
            // 将verificationCode与字符串VerificationCode进行比较
            if (verificationCode == VerificationCode)
            {
                // 如果比较结果为true，则执行以下代码
                MaintenancePopover popover = new MaintenancePopover();
                popover.Btn_OK_Click();
                MessageBox.Show("验证码正确，弹窗已消除！", "提示信息", MessageBoxButton.OK);
                return true;
            }
            else
            {
                MessageBox.Show("输入的验证码不正确！", "提示信息", MessageBoxButton.OK);
                return false;
            }
        }

        private void btn_OK_Click(object sender, RoutedEventArgs e)
        {
            Close();
            Result?.Invoke(this, new MessageBoxEventArgs() { Result = new MessageResult() { IsYes = true } });
        }

    }
}
