using System.ComponentModel.DataAnnotations;

namespace ClinicaVistaalegre.Shared.Models
{
    public class CitaDateValidation : ValidationAttribute
    {
        public CitaDateValidation() { }

        public string GetErrorMessage() => "La fecha debe ser posterior a hoy";

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var date = (DateTime)value;

            if (DateTime.Compare(date, DateTime.Now) < 0) return new ValidationResult(GetErrorMessage());
            else return ValidationResult.Success;
        }
    }
}
