using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

//Might have to delete later 
namespace Web_Asg.Models
{
    public class AreaInterestViewModel
    {
        [Key]
        [Display(Name = "ID")]
        public int AreaInterestID { get; set; }

        [Display(Name = "Area of Interest")]
        [Required]
        public string Name { get; set; }
        [Display(Name = "Competition No: ")]
        [Required]
        public int CompNo { get; set; }
        //public List<AreaInterest> AreaInterestList { get; set; }
        //public List<Competition> compList { get; set; }
        //public AreaInterestViewModel()
        //{
        //    AreaInterestList = new List<AreaInterest>();
        //    compList = new List<Competition>();
        //}
    }
}
