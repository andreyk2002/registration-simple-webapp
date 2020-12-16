using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace registration_simple_webapp.Models.ViewModels
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "Please, specify your email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please, specify your name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please, chose your password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Repeat your password")]
        public string ConfirmPassword { get; set; }

    }
}
