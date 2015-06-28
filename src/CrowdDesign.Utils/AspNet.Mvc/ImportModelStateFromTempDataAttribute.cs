using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CrowdDesign.Utils.AspNet.Mvc
{
    /// <summary>
    /// Represents an attribute that imports the controller's ModelState from another action
    /// </summary>
    public class ImportModelStateFromTempData : ModelStateTempDataTransferAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ModelStateDictionary modelState = filterContext.Controller.TempData[Key] as ModelStateDictionary;

            if (modelState != null)
                filterContext.Controller.ViewData.ModelState.Merge(modelState);

            base.OnActionExecuting(filterContext);
        }
    }
}
