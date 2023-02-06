using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Web_Asg.Models
{
    public class Competition
    {
        [Key]
        [Required]
        [Display(Name = "Competition ID: ")]
        public int CompetitionID { get; set; }

        [Required]
        public int AreaInterestID { get; set; }

        [StringLength(255)]
        [Display(Name = "Name of Competition: ")]
        public string CompetitionName { get; set; }

        [Display(Name = "Start Date of Competition: ")]
        public DateTime StartDate { get; set; }

        [Display(Name = "End Date of Competition: ")]
        public DateTime EndDate { get; set; }

        [Display(Name = "Results Date of Competition: ")]
        public DateTime ResultReleasedDate { get; set; }
    }
}
