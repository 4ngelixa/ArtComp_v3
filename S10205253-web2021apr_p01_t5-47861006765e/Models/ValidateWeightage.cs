using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Web_Asg.DAL;

namespace Web_Asg.Models
{
    public class ValidateWeightage : ValidationAttribute
    {
        private CriteriaDAL criteriaContext = new CriteriaDAL();
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // Get criteria name value to validate
            int addedWeightage = Convert.ToInt32(value);
            // Casting the validation context to the "Criteria" model class
            Criteria criteria = (Criteria)validationContext.ObjectInstance;
            int compID = criteria.CompetitionID;
            int totalweightage = criteriaContext.GetTotalWeightage(compID);
            if (criteriaContext.IsValid(addedWeightage, compID))
                // validation failed
                return new ValidationResult
                ("Total weightage is more than 100. Current total weightage: " + totalweightage);
            else
                // validation passed
                return ValidationResult.Success;
        }
    }
}
