using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Web_Asg.Models
{
    public class CompetitionSubmission
    {
        [Display(Name = "Competition ID")]
        [Required]
        public int CompetitionID { get; set; }

        public int CompetitorID { get; set; }

        [Display(Name = "File Selected")]
        [StringLength(255)]
        // Create validation for file format - only pdf  
        public string FileSubmitted { get; set; }

        [Display(Name = "File Upload Date")]
        [DataType(DataType.DateTime)]
        public DateTime? DateTimeFileUpload { get; set; }

        [StringLength(255)]
        public string Appeal { get; set; }

        public int VoteCount { get; set; }

        public int? Ranking { get; set; }

    }
}
