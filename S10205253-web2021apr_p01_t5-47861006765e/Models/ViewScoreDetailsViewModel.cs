using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web_Asg.Models
{
    public class ViewScoreDetailsViewModel
    {
        public int CompetitionID { get; set; }
        public string CompetitionName { get; set; }
        public Competitor Competitor { get; set; }
        public double TotalScore { get; set; }
        public double JudgeScore { get; set; }
        public double VoteScore { get; set; }
        public string FileShown { get; set; }
        public int Ranking { get; set; }

    }
}
