using System;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.SessionState;
using HP.Autofac.Mvc;
using HP.Autofac.WebApi;
using HP.Core.Caching;
using HP.Core.Configs;
using HP.Core.Dependency;
using HP.Core.Security.Permissions;
using HP.Data.Entity;
using HP.JobSchedulers.Services;
using HP.Web.Mvc.Initialize;
using HPC.BaseService.Contracts;

namespace DF.Web
{
    // 注意: 有关启用 IIS6 或 IIS7 经典模式的说明，
    // 请访问 http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            MvcAutofacIocBuilder.Build<MvcApplication>();
            WebApiAutofacIocBuilder.Build<MvcApplication>();

            //定义缓存驱动
            ICacheProvider provider = new RuntimeMemoryCacheProvider();
            CacheManager.SetProvider(provider, CacheLevel.First);

            //身份认证初始化
            IdentityManager.Initialize(HPConfig.Instance.IdentityProviderType);

            //注册数据库配置
            DatabaseProvider.Register(new DbContextConfig());

            //  DatabaseProvider.Register("","","");
          //  RegisterAllDbConfig();

           // DatabaseProvider.Register(new OracleDbContextConfig());
           // DatabaseProvider.Register(new DbContextConfig1());


            //注册日志
            //Log4NetLoggerAdapter adapter = new Log4NetLoggerAdapter();
            //LogManager.AddLoggerAdapter(adapter);

            //初始化框架
            FrameworkInitializer initializer = new FrameworkInitializer();
            initializer.Initialize();

            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            //启动作业调度
            HPJobSchedulerService.StartScheduler();

            // 添加定时任务，弃用
            // AutoTaskAttribute.RegisterTask();
        }


        protected void Application_End(object sender, EventArgs e)
        {
             //关闭作业调度
            //
            HPJobSchedulerService.ShutdownScheduler();
        }

        //webapi 开启session
        public override void Init()
        {
            this.PostAuthenticateRequest += (sender, e) => HttpContext.Current.SetSessionStateBehavior(SessionStateBehavior.Required);
            base.Init();
        }

        public void RegisterAllDbConfig()
        {
            // var server = IocResolver.Resolve<ITenantInfoContract>();
            //string sql = "SELECT TOP 1 * FROM Base_TenantInfo";
            //using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection("Data Source=DELL-PC;User ID=sa;Password=rockwell123@RA;Initial Catalog=D-Framework;"))
            //{
            //    System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand();
            //    cmd.Connection = conn;
            //    cmd.CommandText = sql;
            //    conn.Open();
            //    System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cmd);
            //    System.Data.DataTable db = new System.Data.DataTable();
            //    da.Fill(db);
            //    if (db.Rows.Count > 0)
            //    {
            //        var row = db.Rows[0];
            //        string conect = "Data Source =.; User ID = " + row["DataBaseUserName"].ToString() + "; Password = " + row["DataBasePassWord"].ToString() + "; Initial Catalog = " + row["SchemaName"].ToString() + "; Max Pool Size = 512; Min Pool Size = 5";
            //        DatabaseProvider.Register("", conect, "System.Data.SqlClient");
            //    }
            //}
            DatabaseProvider.Register("", "Data Source=DESKTOP-71I0RDA;User ID=sa;Password=123456;Initial Catalog=winclient;Max Pool Size=512; Min Pool Size=5", "System.Data.SqlClient");
        }


    }
}