using System.Web.Mvc;

namespace DF.Web.Areas.BaseApi
{
    public class BaseApiAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "BaseApi";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "BaseApi_default",
                "BaseApi/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
