
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace PLCServer
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {

            bool IsRun;

            Process[] processes = System.Diagnostics.Process.GetProcessesByName(Application.CompanyName);
           // using (System.Threading.Mutex m=new System.Threading.Mutex (true,Application.ProductName,out IsRun))
            {
                if (processes.Length>1)
                {
                    MessageBox.Show("目前已有一个设备控制服务程序正在运行,请勿重复运行程序");
                    System.Threading.Thread.Sleep(1000);
                    System.Environment.Exit(1);
                    return;
                }
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);


          //  var builder = new ContainerBuilder();

          //  string[] array = new string[7];
          //  string path = Application.StartupPath;
          //  array[0] = path + "/HP.Core.dll";
          //  array[1] = path + "/HP.Data.Entity.dll";
          //  array[2] = path + "/HP.Data.Orm.dll";
          //  array[3] = path + "/HPC.BaseService.dll";
          //  array[4] = path + "/HP.Utility.dll";
          //  array[5] = path + "/log4net.dll";
          //  array[6] = path + "/HP.Web.Mvc.dll";
          //  builder.RegisterServices(array);
          ////DatabaseProvider.Register(new DbContextConfig());
          //  Autofac.IContainer container = IocBuilder.Build(builder);
          //  ServiceLocator.SetLocatorProvider(() => new AutofacServiceLocator(container));

            Application.Run(new MainForm());
        }
    }
}
