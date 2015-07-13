using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CrowdDesign.Utils.AspNet.Mvc
{
    public abstract class ModelStateTempDataTransferAttribute : ActionFilterAttribute
    {
        protected static readonly string Key = typeof(ModelStateTempDataTransferAttribute).FullName;
    }
}
