using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrowdDesign.Core.Entities
{
    public class User
    {
        #region Properties
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool IsAdmin { get; set; }
        public ICollection<Sketch> Sketches { get; set; }
        #endregion
    }
}
