using System.Web.Mvc;
using System.Web.Security;
using CrowdDesign.Core.Entities;
using CrowdDesign.Core.Interfaces.Repositories;
using CrowdDesign.Infrastructure.SQLServer.Contexts;
using CrowdDesign.Infrastructure.SQLServer.Repositories;
using CrowdDesign.UI.Web.Models;

namespace CrowdDesign.UI.Web.Controllers
{
    public class SecurityController : BaseController<IUserRepository, User, int>
    {
        #region Constructors
        public SecurityController()
            : base(new UserRepository(new DatabaseContext()))
        {
        }
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
                User user = Repository.Login(viewModel.Username, viewModel.Password);

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
        #endregion
    }
}
