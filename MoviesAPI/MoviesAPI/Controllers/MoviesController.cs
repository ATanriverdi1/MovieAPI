using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesAPI.DTOs.Movie;
using MoviesAPI.Entities;
using MoviesAPI.Entities.EntityContext;
using MoviesAPI.Helpers;
using MoviesAPI.Services;
using MoviesAPI.Validators;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Microsoft.Extensions.Logging;

namespace MoviesAPI.Controllers
{
    [ApiController]
    [Route("api/movies")]
    public class MoviesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly MovieValidator _validationRules;
        private readonly IFileStorageService _storageService;
        private readonly ILogger<MoviesController> _logger;
        private readonly string containerName = "movie";

        public MoviesController(ApplicationDbContext context,
            IMapper mapper, MovieValidator validationRules, IFileStorageService storageService,
            ILogger<MoviesController> logger)
        {
            this._context = context;
            this._mapper = mapper;
            this._validationRules = validationRules;
            this._storageService = storageService;
            this._logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IndexMoviePageDTO>> Get()
        {
            var top = 6;
            var today = DateTime.Today;
            var upcomingReleases = await _context.Movies
                .Where(x => x.ReleaseDate > today)
                .OrderBy(x => x.ReleaseDate)
                .Take(top)
                .ToListAsync();

            var inTheaters = await _context.Movies
                .Where(x => x.InTheaters)
                .Take(top)
                .ToListAsync();

            var result = new IndexMoviePageDTO();
            result.InTheathers = _mapper.Map<List<MovieDTO>>(inTheaters);
            result.UpcomingReleases = _mapper.Map<List<MovieDTO>>(upcomingReleases);

            return result;
        }

        [HttpGet("filter")]
        public async Task<ActionResult<List<MovieDTO>>> Filter([FromQuery] FilterMoviesDTO filterMoviesDTO)
        {
            var moviesQueryable = _context.Movies.AsQueryable();
            if (!string.IsNullOrWhiteSpace(filterMoviesDTO.Title))
            {
                moviesQueryable = moviesQueryable.Where(x => x.Title.Contains(filterMoviesDTO.Title));
            }
            if (filterMoviesDTO.InTheaters)
            {
                moviesQueryable = moviesQueryable.Where(x => x.InTheaters);
            }
            if (filterMoviesDTO.UpcomingReleases)
            {
                var today = DateTime.Today;
                moviesQueryable = moviesQueryable.Where(x => x.ReleaseDate > today);
            }
            if (filterMoviesDTO.GenreId != 0)
            {
                moviesQueryable = moviesQueryable
                    .Where(x => x.MoviesGenres.Select(y => y.GenreId)
                    .Contains(filterMoviesDTO.GenreId));
            }
            if (!string.IsNullOrWhiteSpace(filterMoviesDTO.OrderingField))
            {
                try
                {
                    moviesQueryable = moviesQueryable.OrderBy($"{filterMoviesDTO.OrderingField} {(filterMoviesDTO.AscendingOrder ? "ascending" : "descending")} ");
                }
                catch (Exception)
                {
                    _logger.LogWarning("Could not order by field: " + filterMoviesDTO.OrderingField);
                }
                
            }

            await HttpContext.InsertPaginationParametersInResponse(moviesQueryable, filterMoviesDTO.RecordsPerPage);

            var movies = await moviesQueryable.Paginate(filterMoviesDTO.Pagination).ToListAsync();

            return _mapper.Map<List<MovieDTO>>(movies);
        }

        [HttpGet("{id:int}", Name = "getMovie")]
        public async Task<ActionResult<MovieDetailsDTO>> Get(int id)
        {
            var movie = await _context.Movies
                .Include(x => x.MoviesGenres).ThenInclude(x => x.Genre)
                .Include(x => x.MoviesActors).ThenInclude(x => x.Person)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (movie == null)
            {
                return NotFound();
            }

            return _mapper.Map<MovieDetailsDTO>(movie);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromForm] MovieCreationDTO movieCreationDTO)
        {
            var movie = _mapper.Map<Movie>(movieCreationDTO);
            var validate = _validationRules.Validate(movieCreationDTO);
            if (!validate.IsValid)
            {
                return BadRequest();
            }
            if (movieCreationDTO.Poster != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await movieCreationDTO.Poster.CopyToAsync(memoryStream);
                    var content = memoryStream.ToArray();
                    var extension = Path.GetExtension(movieCreationDTO.Poster.FileName);
                    movie.Poster = await _storageService.SaveFile(content, extension, containerName, movieCreationDTO.Poster.ContentType);
                }
            }

            AnnotateActorsOrder(movie);
            _context.Add(movie);
            await _context.SaveChangesAsync();
            var movieDTO = _mapper.Map<MovieDTO>(movie);
            return new CreatedAtRouteResult("getMovie", new { id = movie.Id }, movieDTO);
        }

        private static void AnnotateActorsOrder(Movie movie)
        {
            if (movie.MoviesActors != null)
            {
                for (int i = 0; i < movie.MoviesActors.Count; i++)
                {
                    movie.MoviesActors[i].Order = i;
                }
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, [FromForm] MovieCreationDTO movieCreationDTO)
        {
            var movieDb = await _context.Movies.FirstOrDefaultAsync(x => x.Id == id);
            if (movieDb == null)
            {
                return NotFound();
            }
            var validateResult = _validationRules.Validate(movieCreationDTO);
            if (!validateResult.IsValid)
            {
                return BadRequest();
            }
            movieDb = _mapper.Map(movieCreationDTO, movieDb);
            if (movieCreationDTO.Poster != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await movieCreationDTO.Poster.CopyToAsync(memoryStream);
                    var content = memoryStream.ToArray();
                    var extension = Path.GetExtension(movieCreationDTO.Poster.FileName);
                    movieDb.Poster = await _storageService.EditFile(content, extension, containerName, movieDb.Poster, movieCreationDTO.Poster.ContentType);
                }
            }
            await _context.Database.ExecuteSqlInterpolatedAsync($"delete from moviesActors where MovieId = {movieDb.Id}; delete from moviesGenres where MovieId = {movieDb.Id}");
            AnnotateActorsOrder(movieDb);
            await _context.SaveChangesAsync();
            return Ok("Modifield Success");
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult> Patch(int id, [FromBody] JsonPatchDocument<MoviePatchDTO> patchDocument)
        {
            if (patchDocument == null)
            {
                return BadRequest();
            }

            var movieDb = await _context.Movies.FirstOrDefaultAsync(x => x.Id == id);
            if (movieDb == null)
            {
                return NotFound();
            }

            var movieDTO = _mapper.Map<MoviePatchDTO>(movieDb);
            patchDocument.ApplyTo(movieDTO, ModelState);
            var isValid = TryValidateModel(movieDTO);
            if (!isValid)
            {
                return BadRequest(ModelState);
            }
            _mapper.Map(movieDTO, movieDb);
            await _context.SaveChangesAsync();
            return Ok("success");
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var movie = await _context.Movies.AnyAsync(x => x.Id == id);
            if (!movie)
            {
                return NotFound();
            }
            _context.Remove(new Movie() { Id = id });
            await _context.SaveChangesAsync();
            return Ok("Movie is Delete");
        }
    }
}
