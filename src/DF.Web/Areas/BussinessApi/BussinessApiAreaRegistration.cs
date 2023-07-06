using System.Web.Mvc;

namespace DF.Web.Areas.BussinessApi
{
    public class BussinessApiAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "BussinessApi";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "BussinessApi_default",
                "BussinessApi/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
