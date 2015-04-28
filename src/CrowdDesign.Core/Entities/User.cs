using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrowdDesign.Core.Entities
{
    /// <summary>
    /// Represents a user of the system
    /// </summary>
    public class User
    {
        #region Properties
        /// <summary>
        /// Gets or sets the user id.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Gets or sets the user name.
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// Gets or sets the user password.
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// Gets or sets a value that indicates if the user is an administrator.
        /// </summary>
        public bool IsAdmin { get; set; }
        /// <summary>
        /// Gets or sets the collection of solutions sketches authored by the user.
        /// </summary>
        public ICollection<Sketch> Sketches { get; set; }
        #endregion
    }
}
