using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesAPI.DTOs.Movie;
using MoviesAPI.Entities;
using MoviesAPI.Entities.EntityContext;
using MoviesAPI.Services;
using MoviesAPI.Validators;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

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
        private readonly string containerName = "movie";

        public MoviesController(ApplicationDbContext context,
            IMapper mapper, MovieValidator validationRules, IFileStorageService storageService)
        {
            this._context = context;
            this._mapper = mapper;
            this._validationRules = validationRules;
            this._storageService = storageService;
        }

        [HttpGet]
        public async Task<ActionResult<List<MovieDTO>>> Get()
        {
            var movie = await _context.Movies.ToListAsync();
            return _mapper.Map<List<MovieDTO>>(movie);
        }

        [HttpGet("{id:int}", Name = "getMovie")]
        public async Task<ActionResult<MovieDTO>> Get(int id)
        {
            var movie = await _context.Movies.FirstOrDefaultAsync(x => x.Id == id);

            if (movie == null)
            {
                return NotFound();
            }

            return _mapper.Map<MovieDTO>(movie);
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
            if (movie.Poster != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await movieCreationDTO.Poster.CopyToAsync(memoryStream);
                    var content = memoryStream.ToArray();
                    var extension = Path.GetExtension(movieCreationDTO.Poster.FileName);
                    movie.Poster = await _storageService.SaveFile(content, extension, containerName, movieCreationDTO.Poster.ContentType);
                }
            }
            _context.Add(movie);
            await _context.SaveChangesAsync();
            var movieDTO = _mapper.Map<MovieDTO>(movie);
            return new CreatedAtRouteResult("getMovie", new { id = movie.Id }, movieDTO);
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
