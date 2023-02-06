using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web_Asg.Models
{
    public class ViewSubmissionListViewModel
    {
        public int CompetitionID { get; set; }
        public string CompetitionName { get; set; }
        public List<Competitor> competitorList { get; set; }

        public ViewSubmissionListViewModel()
        {
            competitorList = new List<Competitor>();
        }
    }
}
