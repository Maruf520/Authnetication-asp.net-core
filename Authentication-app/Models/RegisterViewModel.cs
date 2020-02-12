using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Authentication_app.Models
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Display(Name="Confirm Password")]
        [Compare("Password",ErrorMessage="Password and COnfirm Password do not match")]
        public string ConfirmPassword { get; set; }
    }
}
