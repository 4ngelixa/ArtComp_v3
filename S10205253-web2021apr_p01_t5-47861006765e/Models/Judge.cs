using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Web_Asg.Models
{
    public class Judge
    {
        [Required]
        [Display(Name = "Judge ID: ")]
        public int JudgeID { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Judge Name ")]
        public string JudgeName { get; set; }

        [Display(Name = "Salutation")]
        public string Salutation { get; set; }

        public int AreaInterestID { get; set; }

        [Required]
        [EmailAddress]
        [ValidateEmailExists]
        [Display(Name = "Email Address ")]
        public string EmailAddr { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(255)]
        public string Password { get; set; }

    }
}
