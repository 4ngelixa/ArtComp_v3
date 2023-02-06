using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web_Asg.Models
{
    public class ViewScoreViewModel
    {
        public int CompetitionID { get; set; }
        public string CompetitionName { get; set; }
        public List<Competitor> competitorDoneList { get; set; }
        public List<Competitor> competitorNotDoneList { get; set; }
        public List<double> competitionDoneScoreList { get; set; }
        public List<double> judgeScoreList { get; set; }
        public List<double> voteScoreList { get; set; }

        public ViewScoreViewModel()
        {
            competitionDoneScoreList = new List<double>();
            competitorDoneList = new List<Competitor>();
            competitorNotDoneList = new List<Competitor>();
            judgeScoreList = new List<double>();
            voteScoreList = new List<double>();
        }
    }
}
