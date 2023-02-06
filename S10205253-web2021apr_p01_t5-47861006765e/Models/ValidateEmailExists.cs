using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Web_Asg.DAL;

namespace Web_Asg.Models
{
    public class ValidateEmailExists : ValidationAttribute
    {
        private JudgeDAL judgeContext = new JudgeDAL();
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // Get the email value to validate
            string email = Convert.ToString(value);
            // Casting the validation context to the "Judge" model class
            Judge judge = (Judge)validationContext.ObjectInstance;
            // Get the Judge Id from the staff instance
            int judgeId = judge.JudgeID;
            if (judgeContext.IsEmailExist(email, judgeId))
                // validation failed
                return new ValidationResult
                ("Email address is already in use!");
            else
                // validation passed
                return ValidationResult.Success;
        }

    }
}
