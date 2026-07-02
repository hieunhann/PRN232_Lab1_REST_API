using FluentValidation;
using PRN232.LAB_2_REST_API.Services.Models.Requests;

namespace PRN232.LAB_2_REST_API.Services.Validators
{
    /// <summary>
    /// FluentValidation validator for StudentRequest.
    /// Implements advanced validation with custom rules following FPTU standards.
    /// </summary>
    public class StudentRequestValidator : AbstractValidator<StudentRequest>
    {
        public StudentRequestValidator()
        {
            // ─── FullName ────────────────────────────────────────────────────
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("FullName is required.")
                .Length(2, 100).WithMessage("FullName must be between 2 and 100 characters.")
                .Matches(@"^[\p{L}\s]+$").WithMessage("FullName can only contain letters and spaces.");

            // ─── Email ───────────────────────────────────────────────────────
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Email address is not in a valid format.")
                .MaximumLength(100).WithMessage("Email must not exceed 100 characters.");

            // ─── DateOfBirth ─────────────────────────────────────────────────
            RuleFor(x => x.DateOfBirth)
                .NotEmpty().WithMessage("DateOfBirth is required.")
                .LessThan(DateTime.Today).WithMessage("Date of birth must be in the past.")
                .GreaterThan(new DateTime(1900, 1, 1)).WithMessage("Date of birth is not valid.");

            // ─── StudentCode (Custom FPTU Rule) ──────────────────────────────
            // FPTU StudentCode pattern: SE19886, CE18793, IA20001, AI22123, ...
            // Format: 2 uppercase letters + 5 digits
            RuleFor(x => x.StudentCode)
                .Must(code => string.IsNullOrEmpty(code) || IsFptuStudentCode(code))
                .WithMessage("StudentCode must follow FPTU format (e.g. SE19886, CE18793, IA20001). Format: 2 uppercase letters + 5 digits.")
                .When(x => !string.IsNullOrEmpty(x.StudentCode));
        }

        /// <summary>
        /// Custom validation rule: validates StudentCode against FPTU standard.
        /// Valid format: 2 uppercase letters + 5 digits (e.g. SE19886, CE18793, IA20001).
        /// </summary>
        private static bool IsFptuStudentCode(string? code)
        {
            if (string.IsNullOrWhiteSpace(code)) return false;
            return System.Text.RegularExpressions.Regex.IsMatch(code, @"^[A-Z]{2}\d{5}$");
        }
    }
}
