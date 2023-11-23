using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;


namespace TwitterClone.Utils
{
    public class DOBAttribute : ValidationAttribute
    {


        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is DateTime date)
            {
                DateTime latest = DateTime.Today.AddYears(-16);
                DateTime oldest = DateTime.Today.AddYears(-120);

                if (date <= latest & date >= oldest)
                {
                    return ValidationResult.Success;
                }
            }

            return new ValidationResult($"You must enter your real date of birth and be at least 16 years old.");
        }

    }
}