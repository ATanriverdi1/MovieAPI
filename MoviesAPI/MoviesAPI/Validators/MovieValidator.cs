using FluentValidation;
using MoviesAPI.DTOs.Movie;

namespace MoviesAPI.Validators
{
    public class MovieValidator : AbstractValidator<MovieCreationDTO>
    {
        public MovieValidator()
        {
            RuleFor(x => x.Title).NotEmpty().WithMessage("Gerekli Alan")
                                 .Length(1, 300).WithMessage("Min 1 ile Max 300 karaketer arasında olmalıdır");
            
            RuleFor(x=>x.Summary).NotEmpty().WithMessage("Gerekli Alan")
                                 .Length(1, 300).WithMessage("Min 1 ile Max 300 karaketer arasında olmalıdır");

            RuleFor(x => x.InTheaters).NotEmpty().NotNull().WithMessage("Gerekli alan");

        }
    }
}
