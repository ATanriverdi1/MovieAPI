using Microsoft.AspNetCore.Mvc;
using MoviesAPI.Entities;
using MoviesAPI.Services;
using MoviesAPI.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesAPI.Controllers
{
    [Route("api/genres")]
    [ApiController]
    public class GenresController : ControllerBase
    {
        private readonly IRepository _repository;
        private readonly GenreValidator _validationRules;

        public GenresController(IRepository repository,
                                GenreValidator validationRules)
        {
            this._repository = repository;
            this._validationRules = validationRules;
        }

        [HttpGet] // api/genres
        [HttpGet("list")] // api/genres/list
        [HttpGet("/allgenres")]
        public async Task<ActionResult<List<Genre>>> Get()
        {
            return await _repository.GetAllGenres();
        }

        [HttpGet("{Id:int}")]
        public ActionResult<Genre> Get(int id, string param2)
        {
            var genre = _repository.GetGenreById(id);

            if (genre == null)
            {
                return NotFound();
            }

            return genre;
        }

        [HttpPost]
        public ActionResult Post([FromBody] Genre genre) 
        {
            var validateResult = _validationRules.Validate(genre);
            if (!validateResult.IsValid)
            {
                return NotFound();
            }
            return Ok(); 
        }
        
        [HttpPut]
        public ActionResult Put([FromBody] Genre genre) 
        {
            return NoContent();
        }

        [HttpDelete]
        public ActionResult Delete([FromBody] Genre genre) 
        {
            return NoContent();
        }
    }
}
