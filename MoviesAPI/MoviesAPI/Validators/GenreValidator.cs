using FluentValidation;
using MoviesAPI.DTOs;

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