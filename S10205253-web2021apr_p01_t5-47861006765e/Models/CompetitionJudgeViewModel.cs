using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Web_Asg.Models
{
    public class AvailJudges
    {
        public string JudgeName { get; set; }
        public int JudgeID { get; set; }
        public bool Selected { get; set; }
    }
    public class CompetitionJudgeViewModel
    {
        [Required]
        public int CompetitionID { get; set; }

        public List<AvailJudges> Judges { get; set; }

        public List<int> JudgeID { get; set; }


    }
}
