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

                if (date <= latest)
                {
                    return ValidationResult.Success;
                }
            }

            return new ValidationResult($"You must be at least 16 years old to register.");
        }

    }
}