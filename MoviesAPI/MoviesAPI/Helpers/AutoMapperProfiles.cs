using AutoMapper;
using MoviesAPI.DTOs;
using MoviesAPI.DTOs.Movie;
using MoviesAPI.DTOs.Person;
using MoviesAPI.Entities;
using System.Collections.Generic;


namespace MoviesAPI.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Genre, GenreDTO>().ReverseMap();
            CreateMap<GenreCreationDTO, Genre>();

            CreateMap<Person, PersonDTO>().ReverseMap();
            CreateMap<PersonCreationDTO, Person>()
                .ForMember(x => x.Picture, options => options.Ignore());
            CreateMap<Person, PersonPatchDTO>().ReverseMap();

            CreateMap<Movie, MovieDTO>().ReverseMap();
            CreateMap<MovieCreationDTO, Movie>()
                .ForMember(x => x.Poster, options => options.Ignore())
                .ForMember(x => x.MoviesGenres, options => options.MapFrom(MapMoviesGenres))
                .ForMember(x => x.MoviesActors, options => options.MapFrom(MapMoviesActors));

            CreateMap<Movie, MovieDetailsDTO>()
                .ForMember(x => x.Poster, options => options.Ignore())
                .ForMember(x => x.Genres, options => options.MapFrom(MapMoviesGenres))
                .ForMember(x => x.Actors, options => options.MapFrom(MapMoviesActors));

            CreateMap<Movie, MoviePatchDTO>().ReverseMap();

        }


        private List<GenreDTO> MapMoviesGenres(Movie movie, MovieDetailsDTO movieDetailsDTO)
        {
            var result = new List<GenreDTO>();
            foreach (var moviegenre in movie.MoviesGenres)
            {
                result.Add(new GenreDTO() { Id = moviegenre.GenreId, Name = moviegenre.Genre.Name });
            }

            return result;
        }

        private List<ActorDTO> MapMoviesActors(Movie movie, MovieDetailsDTO movieDetailsDTO)
        {
            var result = new List<ActorDTO>();
            foreach (var actor in movie.MoviesActors)
            {
                result.Add(new ActorDTO() { PersonId = actor.PersonId, PersonName = actor.Person.Name, Character = actor.Character });
            }

            return result;
        }




        private List<MoviesGenre> MapMoviesGenres(MovieCreationDTO movieCreation, Movie movie)
        {
            var result = new List<MoviesGenre>();
            foreach (var id in movieCreation.GenresIds)
            {
                result.Add(new MoviesGenre() { GenreId = id });
            }

            return result;
        }

        private List<MoviesActors> MapMoviesActors(MovieCreationDTO movieCreation, Movie movie)
        {
            var result = new List<MoviesActors>();
            foreach (var actor in movieCreation.Actors)
            {
                result.Add(new MoviesActors() { Character = actor.Character, PersonId = actor.PersonId });
            }

            return result;
        }
    }
}