using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using wms.Client.Common;
using wms.Client.View;
using wms.Client.ViewDlg;
using wms.Client.ViewModel;
using wms.Client.LogicCore.Common;
using wms.Client.Core.share.DataInterfaces;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;

namespace wms.Client.View
{
    /// <summary>
    /// InTaskView.xaml 的交互逻辑
    /// </summary>
    public partial class ManualInView : UserControl
    {
        public ManualInView()
        {
            InitializeComponent();
        }

    }
}
