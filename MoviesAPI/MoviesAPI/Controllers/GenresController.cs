using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class GenresController : ControllerBase
    {
        private readonly IRepository _repository;
        private readonly GenreValidator _validationRules;
        private readonly ILogger<GenresController> _logger;

        public GenresController(IRepository repository,
                                GenreValidator validationRules,
                                ILogger<GenresController> logger)
        {
            this._repository = repository;
            this._validationRules = validationRules;
            this._logger = logger;
        }

        [HttpGet] // api/genres
        [HttpGet("list")] // api/genres/list
        [HttpGet("/allgenres")]
        //[ResponseCache(Duration = 60)]
        public async Task<ActionResult<List<Genre>>> Get()
        {
            _logger.LogDebug("Get all genres");
            return await _repository.GetAllGenres();
        }

        [HttpGet("{Id:int}")]
        public ActionResult<Genre> Get(int id, string param2)
        {
            var genre = _repository.GetGenreById(id);

            if (genre == null)
            {
                _logger.LogWarning("{id} not found");
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
