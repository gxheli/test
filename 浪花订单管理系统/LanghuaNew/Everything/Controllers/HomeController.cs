using System.Web.Mvc;

namespace Everything.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            string url = Request.Url.ToString();
            if (!url.Contains("localhost"))
            {
                return HttpNotFound();
            }
            return View();
        }
    }
}