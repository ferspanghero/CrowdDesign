using System.Web.Mvc;

namespace CrowdDesign.UI.Web.Controllers
{
    public class SketchController : Controller
    {
        //
        // GET: /Sketch/

        public ActionResult Index()
        {
            return View("IndexSketch");
        }

    }
}
