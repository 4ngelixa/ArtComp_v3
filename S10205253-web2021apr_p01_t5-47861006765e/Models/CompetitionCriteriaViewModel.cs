using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web_Asg.Models
{
    public class CompetitionCriteriaViewModel
    {
        public List<Competition> competitionDoneList { get; set; }
        public List<Competition> competitionNotDoneList { get; set; }
        public List<Criteria> criteriaList { get; set; }

        public CompetitionCriteriaViewModel()
        {
            competitionDoneList = new List<Competition>();
            competitionNotDoneList = new List<Competition>();
            criteriaList = new List<Criteria>();
        }
    }
}
