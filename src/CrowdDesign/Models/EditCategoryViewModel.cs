using CrowdDesign.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CrowdDesign.UI.Web.Models
{
    public class EditCategoryViewModel
    {
        #region Properties
        public int? ProjectId { get; set; }
        public int? CategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        #endregion        
    }
}