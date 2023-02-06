using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Web_Asg.Models
{
    public class ViewSubmissionViewModel
    {
        [Display(Name = "Competition Name")]
        public string CompetitionName { get; set; }

        [Display(Name = "Competition ID")]
        public int CompetitionID { get; set; }

        public int CompetitorID { get; set; }

        public int CriteriaID { get; set; }
        public string CompetitorName { get; set; }

        [Display(Name = "File Selected")]
        public string fileShown { get; set; }

        [Required(ErrorMessage = "A value is required")]
        [Range(0, 10, ErrorMessage = "Enter a value from 0-10")]
        public int Score { get; set; }

        public List<String> scoreList { get; set; }

        public List<Criteria> criteriaList { get; set; }


    }
}