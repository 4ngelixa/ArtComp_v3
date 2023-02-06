using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Web_Asg.Models
{
    public class AreaInterest
    {
        [Key]
        [Display(Name = "ID")]
        public int AreaInterestID { get; set; }

        [Display(Name = "Area of Interest")]
        [Required]
        public string Name { get; set; }

    }
}
