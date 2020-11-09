using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
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
    [EnableCors(PolicyName = "AllowAPIRequestIO")]
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

        private void GenerateLinks(GenreDTO genreDTO)
        {
            genreDTO.Links.Add(new Link(Url.Link("getGenre", new { Id = genreDTO.Id }), "get-genre", method: "GET"));
            genreDTO.Links.Add(new Link(Url.Link("putGenre", new { Id = genreDTO.Id }), "put-genre", method: "PUT"));
            genreDTO.Links.Add(new Link(Url.Link("deleteGenre", new { Id = genreDTO.Id }), "delete-genre", method: "DELETE"));
        }

        [HttpGet(Name = "getGenres")] // api/genres
        //[ResponseCache(Duration = 60)]
        public async Task<IActionResult> Get(bool includeHATEOAS = true)
        {
            var genres = await _context.Genres.AsNoTracking().ToListAsync();
            var genresDTOs = _mapper.Map<List<GenreDTO>>(genres);
            if (includeHATEOAS)
            {
                var resourceCollection = new ResourceCollection<GenreDTO>(genresDTOs);
                genresDTOs.ForEach(genre => GenerateLinks(genre));
                resourceCollection.Links.Add(new Link(Url.Link("getGenres", new { }), "get-genres", method: "GET"));
                resourceCollection.Links.Add(new Link(Url.Link("createGenre", new { }), "create-genre", method: "POST"));

                return Ok(resourceCollection);
            }
            return Ok(genresDTOs);
        }

        [HttpGet("{Id:int}", Name = "getGenre")]
        public async Task<ActionResult<GenreDTO>> Get(int id, bool includeHATEOAS = true)
        {
            var genre = await _context.Genres.FirstOrDefaultAsync(x => x.Id == id);

            if (genre == null)
            {
                return NotFound();
            }

            var genreDTO = _mapper.Map<GenreDTO>(genre);

            if (includeHATEOAS)
            {
                GenerateLinks(genreDTO);
            }

            return genreDTO;
        }

        [HttpPost(Name = "createGenre")]
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

        [HttpPut("{Id:int}", Name = "putGenre")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
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

        [HttpDelete("{Id:int}", Name = "DeleteGenre")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles ="Admin")]
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