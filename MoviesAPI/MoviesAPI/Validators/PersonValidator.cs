using FluentValidation;
using MoviesAPI.DTOs.Person;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesAPI.Validators
{
    public class PersonValidator : AbstractValidator<PersonCreationDTO>
    {
        public PersonValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Gerekli alan")
                .Length(1, 20).WithMessage("Min {0} ile max {1} karakter içermelidir");

            RuleFor(x => x.Biography)
                .NotEmpty().WithMessage("Gerekli alan")
                .Length(1, 100).WithMessage("Bu alan boş bırakılamaz.");

            RuleFor(x => x.DateOfBirth)
                .Must(BeAValidDate)
                .WithMessage("Geçersiz giriş");
        }

        private bool BeAValidDate(DateTime dateTime)
        {
            return !dateTime.Equals(default(DateTime));
        }
    }
}
