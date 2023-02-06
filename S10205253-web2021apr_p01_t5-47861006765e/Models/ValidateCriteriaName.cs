using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Web_Asg.DAL;

namespace Web_Asg.Models
{
    public class ValidateCriteriaName : ValidationAttribute
    {
        private CriteriaDAL criteriaContext = new CriteriaDAL();
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // Get weightage value to validate

            string criteriaName = value.ToString();
            // Casting the validation context to the "Criteria" model class
            Criteria criteria = (Criteria)validationContext.ObjectInstance;
            int competitionID = criteria.CompetitionID;
            if (criteriaContext.IsValid(criteriaName, competitionID))
                // validation failed
                return new ValidationResult
                ("Criteria Name is repeated.");
            else
                // validation passed
                return ValidationResult.Success;
        }
    }
}
