using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesAPI.DTOs.Person;
using MoviesAPI.Entities;
using MoviesAPI.Entities.EntityContext;
using MoviesAPI.Validators;

namespace MoviesAPI.Controllers
{
    [Route("api/people")]
    [ApiController]
    public class PeopleController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly PersonValidator _validationRules;

        public PeopleController(ApplicationDbContext context,
                                IMapper mapper,
                                PersonValidator validationRules)
        {
            this._context = context;
            this._mapper = mapper;
            this._validationRules = validationRules;
        }

        [HttpGet]
        public async Task<ActionResult<List<PersonDTO>>> Get() 
        {
            var people = await _context.People.AsNoTracking().ToListAsync();

            return _mapper.Map<List<PersonDTO>>(people);
        }

        [HttpGet("{Id:int}", Name = "getPerson")]
        public async Task<ActionResult<PersonDTO>> Get(int id) 
        {
            var person = await _context.People.FirstOrDefaultAsync(x => x.Id == id);
            
            if (person == null)
            {
                return NotFound();
            }

            return _mapper.Map<PersonDTO>(person);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromForm] PersonCreationDTO personCreation) 
        {
            var validateResult = _validationRules.Validate(personCreation);
            if (validateResult.IsValid)
            {
                var person = _mapper.Map<Person>(personCreation);
                _context.Add(person);
                //await _context.SaveChangesAsync();
                var personDTO = _mapper.Map<PersonDTO>(person);
                return new CreatedAtRouteResult("getPerson", new { Id = personDTO.Id }, personDTO);
            }
            return NotFound();
        }

        [HttpPut("{Id:int}")]
        public async Task<ActionResult> Put(int id, [FromBody] PersonCreationDTO personCreation)
        {
            var validateResult = _validationRules.Validate(personCreation);
            if (validateResult.IsValid)
            {
                var person = _mapper.Map<Person>(personCreation);
                person.Id = id;
                _context.Entry(person).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok("modifield success");
            }
            return NotFound();
        }

        [HttpDelete("{Id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var exists = await _context.People.AnyAsync(x => x.Id == id);
            if (!exists)
            {
                return NotFound();
            }
            _context.Remove(new Person() { Id = id });
            await _context.SaveChangesAsync();

            return Ok("Person is delete");
        }
    }
}
