using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace registration_simple_webapp.Models.ViewModels
{
    public class CheckboxListModel
    {
        [Required]
        [Display(Name = "Selected")]
        public List<Boolean> Selected { get; set; }
    }
}
