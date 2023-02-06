using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Web_Asg.Models
{
    public class Comment
    {
        [Required]
        public int CommentID { get; set; }

        public int CompetitionID { get; set; }

        [StringLength(255)]
        [Display(Name = "Description: ")]
        public string Description { get; set; }

        [Display(Name = "Date Posted: ")]
        public DateTime DateTimePosted { get; set; }
    }
}
