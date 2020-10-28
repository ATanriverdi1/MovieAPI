using FluentValidation;
using MoviesAPI.DTOs;
using MoviesAPI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;

namespace MoviesAPI.Validators
{
    public class GenreValidator : AbstractValidator<GenreCreationDTO>
    {
        public GenreValidator() 
        {
            RuleFor(x => x.Name).NotNull()
                                .Length(1, 10)
                                .WithMessage("Bu Kısım Boş Geçilemez");
            
        }
    }
}
