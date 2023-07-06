using System;
using System.Windows.Controls;
using System.Windows;

namespace wms.Client.Template
{
    /// <summary>
    /// MsgView.xaml 的交互逻辑
    /// </summary>
    public partial class MsgView : UserControl
    {
        public MsgView()
        {
            InitializeComponent();
        }
        public MsgView(string mess)
        {
            InitializeComponent();
            Msg.Text = mess;
        }
    }
}
