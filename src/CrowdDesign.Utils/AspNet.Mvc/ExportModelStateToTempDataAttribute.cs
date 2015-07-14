using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CrowdDesign.Utils.AspNet.Mvc
{
    /// <summary>
    /// Represents an attribute that exports the controller's ModelState from one action to another when it is invalid
    /// </summary>
    public class ExportModelStateToTempData : ModelStateTempDataTransferAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            // Checks if the ModelState is invalid
            if (!filterContext.Controller.ViewData.ModelState.IsValid)
            {
                // Only exports the ModelState if the action result is a redirect
                if ((filterContext.Result is RedirectResult) || (filterContext.Result is RedirectToRouteResult))
                {
                    // Stores the ModelState into the Controller's TempData
                    filterContext.Controller.TempData[Key] = filterContext.Controller.ViewData.ModelState;
                }
            }

            base.OnActionExecuted(filterContext);
        }
    }
}
