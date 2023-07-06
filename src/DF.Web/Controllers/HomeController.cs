using System.Web.Mvc;

namespace DF.Web.Controllers
{

    public class HomeController : Controller
    { 
        public ActionResult Index()
        {
            return View();
        }
    }
}
