using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Web_Asg.Models
{
    public class Competitor
    {
        [Required]
        [Display(Name = "Competitor ID")]
        public int CompetitorID { get; set; }

        [Display(Name = "Competitor Name")]
        [StringLength(50)]
        public string CompetitorName { get; set; }

        [Display(Name = "Salutation")]
        [RegularExpression(@"^(Dr|Mrs?|Ms)\. [A-Za-z] ([A - Za - z] (\s|\.|_)?)+[a-zA-Z]*$", ErrorMessage = "Greeting must begin with Mr., Mrs., Ms., or Dr")]
        public string Salutation { get; set; }

        [Display(Name = "Email Address")]
        [EmailAddress]
        public string EmailAddr { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
