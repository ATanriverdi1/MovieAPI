using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MoviesAPI.DTOs;
using MoviesAPI.Entities;
using MoviesAPI.Entities.EntityContext;
using MoviesAPI.Validators;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MoviesAPI.Controllers
{
    [Route("api/genres")]
    [ApiController]
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class GenresController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly GenreValidator _validationRules;
        private readonly ILogger<GenresController> _logger;
        private readonly IMapper _mapper;

        public GenresController(ApplicationDbContext context,
                                GenreValidator validationRules,
                                ILogger<GenresController> logger,
                                IMapper mapper)
        {
            this._context = context;
            this._validationRules = validationRules;
            this._logger = logger;
            this._mapper = mapper;
        }

        [HttpGet] // api/genres
        //[ResponseCache(Duration = 60)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<List<GenreDTO>>> Get()
        {
            var genre = await _context.Genres.AsNoTracking().ToListAsync();
            var genreDTOs = _mapper.Map<List<GenreDTO>>(genre);

            return genreDTOs;
        }

        [HttpGet("{Id:int}", Name = "getGenre")]
        public async Task<ActionResult<GenreDTO>> Get(int id)
        {
            var genre = await _context.Genres.FirstOrDefaultAsync(x => x.Id == id);

            if (genre == null)
            {
                return NotFound();
            }

            var genreDTO = _mapper.Map<GenreDTO>(genre);

            return genreDTO;
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles ="Admin")]
        public async Task<ActionResult> Post([FromBody] GenreCreationDTO genreCreation)
        {
            var validateResult = _validationRules.Validate(genreCreation);

            if (validateResult.IsValid)
            {
                var genre = _mapper.Map<Genre>(genreCreation);
                _context.Add(genre);
                await _context.SaveChangesAsync();
                var genreDTO = _mapper.Map<GenreDTO>(genre);
                return new CreatedAtRouteResult("getGenre", new { Id = genreDTO.Id }, genreDTO);
            }
            return NotFound();
        }

        [HttpPut("{Id:int}")]
        public async Task<ActionResult> Put(int id, [FromBody] GenreCreationDTO genreCreation)
        {
            var validateResult = _validationRules.Validate(genreCreation);
            if (validateResult.IsValid)
            {
                var genre = _mapper.Map<Genre>(genreCreation);
                genre.Id = id;
                _context.Entry(genre).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok("modifield success");
            }

            return NotFound();
        }

        [HttpDelete("{Id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var exists = await _context.Genres.AnyAsync(x => x.Id == id);
            if (!exists)
            {
                return NotFound();
            }
            _context.Remove(new Genre() { Id = id });
            await _context.SaveChangesAsync();

            return Ok("Genre is delete");
        }
    }
}