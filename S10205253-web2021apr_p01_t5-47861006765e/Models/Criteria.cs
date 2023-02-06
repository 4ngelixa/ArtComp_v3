using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Web_Asg.Models
{
    public class Criteria
    {
        [Required]
        public int CriteriaID { get; set; }

        [Required(AllowEmptyStrings = false)]
        public int CompetitionID { get; set; }

        [Required(ErrorMessage = "A name is required")]
        [Display(Name = "Criteria: ")]
        [StringLength(50, ErrorMessage = "Please input less than 50 characters.")]
        [ValidateCriteriaName]
        public string CriteriaName { get; set; }

        [Required(ErrorMessage = "A value is required")]
        [Display(Name = "Weightage")]
        [Range(1, 100, ErrorMessage = "Please enter a value from 1-100")]
        [ValidateWeightage]
        public int Weightage { get; set; }
    }
}
