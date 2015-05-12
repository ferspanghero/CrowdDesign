using System.Web.Mvc;
using System.Web.Security;
using CrowdDesign.Core.Entities;
using CrowdDesign.Core.Interfaces;
using CrowdDesign.Infrastructure.SQLServer.Contexts;
using CrowdDesign.Infrastructure.SQLServer.Repositories;
using CrowdDesign.UI.Web.Models;

namespace CrowdDesign.UI.Web.Controllers
{
    public class SecurityController : Controller
    {
        #region Constructors
        public SecurityController()
        {
            _repository = new SecurityRepository(new DatabaseContext());
        }
        #endregion

        #region Fields
        private readonly ISecurityRepository _repository;
        #endregion

        #region Methods
        public ActionResult Index()
        {
            return View("Login");
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel viewModel)
        {
            if (viewModel == null)
                return View("Error");

            if (ModelState.IsValid)
            {
                User user = _repository.Login(viewModel.Username, viewModel.Password);

                if (user != null && user.Id > 0)
                {
                    FormsAuthentication.SetAuthCookie(viewModel.Username, false);

                    System.Web.HttpContext.Current.Session["userId"] = user.Id;
                    System.Web.HttpContext.Current.Session["userIsAdmin"] = user.IsAdmin;

                    return RedirectToAction("GetProjects", "Project");
                }

                ModelState.AddModelError("Username", "The user name or password is incorrect");
                ModelState.AddModelError("Password", " ");
            }

            return View("Login");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _repository.Dispose();

            base.Dispose(disposing);
        }
        #endregion
    }
}
