using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Web_Asg.Models
{

    // The purpose of the Model is to store Joined Competitions (CompetitionSubmission Objects) -
    // display criterias(with/and weightage) list, results date, and endate
    public class CompsubCriteriaModel
    {
        // Criterias with/and weightage
        public string criterias { get; set; }

        // Competition Submission

        [Display(Name = "Competition ID")]
        public int CompetitionID { get; set; }

        [Display(Name = "Name of Competition: ")]
        public int CompetitorID { get; set; }

        [Display(Name = "File Selected")]
        [StringLength(255)]
        // Create validation for file format - only png, jpeg, gif, pdf, word doc
        public string FileSubmitted { get; set; }

        [Display(Name = "File Upload Date")]
        [DataType(DataType.DateTime)]
        public DateTime? DateTimeFileUpload { get; set; }

        [StringLength(255)]
        public string Appeal { get; set; }
        public int VoteCount { get; set; }
        public int? Ranking { get; set; }

        // Competiton
        public string CompetitionName { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime ResultReleasedDate { get; set; }

    }
}
