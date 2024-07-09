using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Eproject1.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email is a required field")]
        [Display(Name = "Enter Email Address")]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Invalid email format")]
        public string Uemail { get; set; }

        [Required(ErrorMessage = "Password is a required field")]
        [Display(Name = "Enter Password")]
        public string Upwd { get; set; }

    }
}