using FluentValidation;
using MoviesAPI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;

namespace MoviesAPI.Validators
{
    public class GenreValidator : AbstractValidator<Genre>
    {
        public GenreValidator() 
        {
            RuleFor(x => x.Name).NotNull().Length(1, 10).WithMessage("Bu kısım boş geçilemez.");
            RuleFor(x => x.CreditCard).CreditCard().WithMessage("Geçerli bir kart giriniz");
            RuleFor(x => x.Age).InclusiveBetween(18, 120).WithMessage("Yaşınız 18 ile 120 arasında olmalıdır");
            
        }
    }
}
