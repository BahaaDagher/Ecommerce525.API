using System.ComponentModel.DataAnnotations;

namespace Ecommerce525.API.Validations
{
    public class MinMaxCustomeAttribute : ValidationAttribute
    {
        private int minLength ; 
        private int maxLength ; 
        public MinMaxCustomeAttribute (int MinLength , int MaxLength)
        {
            this.minLength = MinLength; 
            this.maxLength= MaxLength; 
        }

        public override string FormatErrorMessage(string name)
        {
            return $"the field {name} must be between {minLength}  , {maxLength}"; 
         }

        public override bool IsValid(object? value)
        { 
            if(value is string name )
            {
                return name.Length > minLength && name.Length < maxLength;  
            }
            return false; 
        }
    }
}
