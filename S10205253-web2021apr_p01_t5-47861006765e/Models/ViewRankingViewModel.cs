using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web_Asg.Models
{
    public class ViewRankingViewModel
    {
        public int CompetitionID { get; set; }
        public string CompetitionName { get; set; }
        public List<Competitor> competitorDoneList { get; set; }
        public List<Competitor> competitorNotDoneList { get; set; }
        public List<double> competitionDoneScoreList { get; set; }
        public List<int> rankingList { get; set; }

        public ViewRankingViewModel()
        {
            competitionDoneScoreList = new List<double>();
            competitorDoneList = new List<Competitor>();
            competitorNotDoneList = new List<Competitor>();
            rankingList = new List<int>();
        }
    }
}
