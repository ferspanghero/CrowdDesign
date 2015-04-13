using CrowdDesign.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CrowdDesign.UI.Web.Models
{
    public class EditSketchViewModel
    {
        #region Properties
        public int? ProjectId { get; set; }
        public int? CategoryId { get; set; }
        public int? SketchId { get; set; }
        public string Data { get; set; }
        public string ImageURI { get; set; }
        #endregion        
    }
}