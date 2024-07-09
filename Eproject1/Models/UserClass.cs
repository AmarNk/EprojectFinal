using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Eproject1.Models
{
    public class UserClass
    {
        [Required(ErrorMessage ="required field")]
        [Display(Name = "Enter Username")]
        [StringLength(maximumLength:10, MinimumLength =3, ErrorMessage = "Username length must been inbetween 3 to 10 characters")]
        public string Uname {  get; set; }

        [Required(ErrorMessage = "required field")]
        [Display(Name = "Enter Email Address")]
        public string Uemail { get; set; }

        [Required(ErrorMessage = "required field")]
        [Display(Name = "Enter password")]
        [DataType(DataType.Password)]

        public string Upwd { get; set; }
        [Required(ErrorMessage = "required field")]
        [Display(Name = "Enter re-password")]
        [DataType(DataType.Password)]
        [Compare("Upwd")]
        public string Repwd {  get; set; }
        [Required(ErrorMessage = "select your department")]
        [Display(Name = "Your Department")]
        public string Department { get; set; }

        [Required(ErrorMessage = "Upload Profile Image")]
        [Display(Name = "Profile Image")]
        public string Uimg { get; set; }

    }
}