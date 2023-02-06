using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;


namespace Web_Asg.Models
{
    public class SubmissionViewModel
    {

        [Display(Name = "Competition ID")]
        [Required]
        public int CompetitionID { get; set; }

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

        public IFormFile fileToUpload { get; set; }

    }
}
