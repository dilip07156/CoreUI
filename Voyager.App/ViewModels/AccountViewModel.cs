using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Voyager.App.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "User Name")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }

        [Required]
        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        public string DateOfBirth { get; set; }
    }

    public class ManageViewModel
    {
        public string Telephone { get; set; }
        public string MobileNumber { get; set; }
        public string FaxNumber { get; set; }
        public string VoyagerUser_Id { get; set; }

        //[DataType(DataType.Url)]
        public string WebSite { get; set; }
        public string command { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        //[Required(ErrorMessage = "Password is Required.")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        //[Required(ErrorMessage = "Confirm Password is Required.")]
        //[Compare("Password",ErrorMessage ="Password not matched.")]
        public string ConfirmPassword { get; set; }

        public bool IsPasswordEnabled { get; set; }

        public string PhotoPath { get; set; }


    }

    public class LoginToken
    {
        public string Token { get; set; }
        
    }

    public class IntegrationLoginViewModel
    {
        [Required]
        [Display(Name = "User Name")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }

        [Required]
        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        public string DateOfBirth { get; set; }

        [Required]
        [Display(Name = "Source")]
        public string Source { get; set; }

        [Required]
        [Display(Name = "Module")]
        public string Module { get; set; }

        [Required]
        [Display(Name = "Operation")]
        public string Operation { get; set; }

        [Required]
        [Display(Name = "Private Key")]
        public string PrivateKey { get; set; }

        [Required]
        [Display(Name = "Key")]
        public string Key { get; set; }

        [Required]
        [Display(Name = "User Key")]
        public string User { get; set; }

        [Required]
        [Display(Name = "target")]
        public string target { get; set; }

        [Required]
        [Display(Name = "ID")]
        public string ID { get; set; }
    }
}

