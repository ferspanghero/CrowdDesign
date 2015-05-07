using System.ComponentModel.DataAnnotations;

namespace CrowdDesign.UI.Web.Models
{
    public class LoginViewModel
    {
        #region Properties
        [Required]
        [StringLength(50)]
        [Display(Name = "User name")]
        public string Username { get; set; }

        [Required]
        [StringLength(50)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        #endregion
    }
}