using FluentValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WestWindLibrary.Entities
{
    //This is just an Example Class to show more Data Annotation - Not part of the Database.
    public class PersonExample
    {
        [Display(Name = "First Name")]
        [Required(ErrorMessage = "You must provide a {0}.")]
        [MaxLength(50, ErrorMessage = "{0} cannot be longer than {1} characters.")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "You must provide a {0}.")]
        [MaxLength(50, ErrorMessage = "{0} cannot be longer than {1} characters.")]
        public string LastName { get; set; }

        [Display(Name = "Phone Number")]
        [Required(ErrorMessage = "You must provide a {0}.")]
        //Validation for Phone numbers, Regex validation provided by Microsoft 
        //This is not the best, but useable
        [Phone(ErrorMessage = "Please enter a valid {0}.")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "You must provide a {0}.")]
        [Range(0, int.MaxValue, ErrorMessage = "{0} must be {1} or greater.")]
        public int Age { get; set; }

        [Display(Name = "SIN")]
        [Required(ErrorMessage = "You must provide a {0}.")]
        [RegularExpression("^\\d{3}-\\d{3}-\\d{3}$", 
            ErrorMessage = "{0} must be in the format ###-###-###")]
        public string SocialInsuranceNumber { get; set; }

        [Required(ErrorMessage = "You must provide a {0}.")]
        [EmailAddress]
        public string Email { get; set; }

        //This is for confirmation of the email entered by the user, so much match the Email
        [Display(Name = "Confirm Email")]
        [Required(ErrorMessage = "You must provide a {0}.")]
        [EmailAddress]
        //Compare the value of one property to another
        //Takes in the property being compared and can provide an ErrorMessage
        [Compare(nameof(Email), ErrorMessage = "{1} and {0} must match.")]
        public string ConfirmEmail { get; set; }

        //Business Rule: Birthday cannot be in the future
        //Use a Custom Validator
        [CustomValidation(typeof(NotFutureValidation), nameof(NotFutureValidation.Validate))]
        public DateOnly Birthday { get; set; }
    }

    //Outside of the PersonExample Class
    public class NotFutureValidation
    {
        //provide a static method that can be used to return the validation results.
        //The Validate Method must have a paramter that is the same datatype
        //as the Property (Birthdate) that we want to validate
        public static ValidationResult? Validate(DateOnly date)
        {
            return date > DateOnly.FromDateTime(DateTime.Today) ? new ValidationResult("The date cannot be in the future.") : ValidationResult.Success;
        }
    }

    //Outside the Class, typically in another file.
    //New Class, inherits the AbstractValidator class, and we tell the AbstractValidator what class it is validating
    public class PersonValidator : AbstractValidator<PersonExample>
    {
        //Create a constructor to hold the validation rules, do not provide it any parameters.
        public PersonValidator()
        {
            //For our rules, we can provide custom errors message for each check
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required.")
                .MaximumLength(50).WithMessage("First name cannot be longer than 50 characters")
                //When method - make it so the proceeding (the validation before When) only happens WHEN the clause is true. In this example, the First Name cannot be equal to John only WHEN LastName is equal to Doe.
                //ApplyConditionTo.CurrentValidator in the WHEN method makes sure it only checks the last rule.
                .NotEqual("John").WithMessage("Your name cannot be John Doe").When(x => x.LastName == "Doe", ApplyConditionTo.CurrentValidator);
            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required.")
                .MaximumLength(50).WithMessage("Last name cannot be longer than 50 characters")
                .NotEqual("Doe").WithMessage("Your name cannot be John Doe").When(x => x.FirstName == "John", ApplyConditionTo.CurrentValidator);
            RuleFor(x => x.Age)
                .NotEmpty().WithMessage("Age is required")
                .GreaterThanOrEqualTo(0).WithMessage("Age must be greater than or equal to 0.");
            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage("Phone number is required.")
                //Matches is the regex matcher.This is a phone number match that include matching international numbers.
                .Matches(@"^(?:\+?\d{1,2}[\s\.\-\u2013]?)?\(?\d{3}\)?[\s\.\-\u2013]?\d{3}[\s\.\-\u2013]?\d{4}$").WithMessage("You must provide a valid phone number.");
            RuleFor(x => x.SocialInsuranceNumber)
                .NotEmpty().WithMessage("A SIN is required.")
                .Matches(@"^\d{3}-\d{3}-\d{3}").WithMessage("SIN must be the format ###-###-###.");
            RuleFor(x => x.Email).NotEmpty().WithMessage("Email is Required").EmailAddress().WithMessage("Not a valid email address format.");
            RuleFor(x => x.ConfirmEmail)
                .NotEmpty().WithMessage("Confirm Email is Required")
                .EmailAddress().WithMessage("Not a valid email address format.")
                .Equal(x => x.Email).WithMessage("Confirm email must match your entered email.");
            RuleFor(x => x.Birthday)
                .NotEmpty().WithMessage("Birthday is required.")
                .GreaterThan(DateOnly.FromDateTime(DateTime.Today.AddYears(-100))).WithMessage("You can't be over 100!");
        }
    }
}