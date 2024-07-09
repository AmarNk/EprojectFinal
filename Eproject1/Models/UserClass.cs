using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Eproject1.Models
{
    public enum Department
    {
        BusinessAdministration = 1,
        ComputerScience = 2,
        MediaScience = 3
    }

    public class UserClass
    {
        [Required(ErrorMessage = "Name is a required field")]
        [Display(Name = "Enter Name")]
        public string Uname { get; set; }

        [Required(ErrorMessage = "Email is a required field")]
        [Display(Name = "Enter Email Address")]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Invalid email format")]
        public string Uemail { get; set; }

        [Required(ErrorMessage = "Password is a required field")]
        [Display(Name = "Enter Password")]
        public string Upwd { get; set; }

        [Required(ErrorMessage = "Repeat Password is a required field")]
        [Display(Name = "Repeat Password")]
        [Compare("Upwd", ErrorMessage = "The password and confirmation password do not match.")]
        public string Repwd { get; set; }

        [Required(ErrorMessage = "Department is a required field")]
        [Display(Name = "Department")]
        public Department Department { get; set; }

        [Display(Name = "Upload Image")]
        public string Uimg { get; set; }
    }
}
