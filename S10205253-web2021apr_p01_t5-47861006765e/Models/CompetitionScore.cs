using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Web_Asg.Models
{
    public class CompetitionScore
    {
        [Required(AllowEmptyStrings = false)]
        public int CriteriaID { get; set; }

        [Required(AllowEmptyStrings = false)]
        public int CompetitorID { get; set; }

        [Required(AllowEmptyStrings = false)]
        public int CompetitionID { get; set; }

        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Score: ")]
        [Range(1, 10)]
        public int Score { get; set; }

        public DateTime DateTimeLastEdit { get; set; }
    }
}
