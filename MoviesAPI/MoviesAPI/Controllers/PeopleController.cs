using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesAPI.DTOs;
using MoviesAPI.DTOs.Person;
using MoviesAPI.Entities;
using MoviesAPI.Entities.EntityContext;
using MoviesAPI.Helpers;
using MoviesAPI.Services;
using MoviesAPI.Validators;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace MoviesAPI.Controllers
{
    [Route("api/people")]
    [ApiController]
    public class PeopleController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly PersonValidator _validationRules;
        private readonly IFileStorageService _fileStorage;
        private readonly string containerName = "people";

        public PeopleController(ApplicationDbContext context,
                                IMapper mapper,
                                PersonValidator validationRules,
                                IFileStorageService fileStorage)
        {
            this._context = context;
            this._mapper = mapper;
            this._validationRules = validationRules;
            this._fileStorage = fileStorage;
        }

        [HttpGet]
        public async Task<ActionResult<List<PersonDTO>>> Get([FromQuery] PaginationDTO paginationDTO)
        {
            var queryable = _context.People.AsQueryable();
            await HttpContext.InsertPaginationParametersInResponse(queryable, paginationDTO.RecordsPerPage);
            var people = await queryable.Paginate(paginationDTO).ToListAsync();
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
        public async Task<ActionResult> Post([FromForm] PersonCreationDTO personCreationDTO)
        {
            var person = _mapper.Map<Person>(personCreationDTO);

            if (personCreationDTO.Picture != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await personCreationDTO.Picture.CopyToAsync(memoryStream);
                    var content = memoryStream.ToArray();
                    var extension = Path.GetExtension(personCreationDTO.Picture.FileName);
                    person.Picture =
                        await _fileStorage.SaveFile(content, extension, containerName,
                                                            personCreationDTO.Picture.ContentType);
                }
            }

            _context.Add(person);
            await _context.SaveChangesAsync();
            var personDTO = _mapper.Map<PersonDTO>(person);
            return new CreatedAtRouteResult("getPerson", new { id = person.Id }, personDTO);
        }

        [HttpPut("{Id:int}")]
        public async Task<ActionResult> Put(int id, [FromForm] PersonCreationDTO personCreation)
        {
            var personDb = await _context.People.FirstOrDefaultAsync(x => x.Id == id);

            if (personDb == null) { return NotFound(); }

            var validateResult = _validationRules.Validate(personCreation);
            if (validateResult.IsValid)
            {
                personDb = _mapper.Map(personCreation, personDb);

                if (personCreation.Picture != null)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await personCreation.Picture.CopyToAsync(memoryStream);
                        var content = memoryStream.ToArray();
                        var extension = Path.GetExtension(personCreation.Picture.FileName);
                        personDb.Picture = await _fileStorage.EditFile(content, extension, containerName, personDb.Picture, personCreation.Picture.ContentType);
                    }
                }
                await _context.SaveChangesAsync();
                return Ok("modifield success");
            }
            return NotFound();
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult> Patch(int id, [FromBody] JsonPatchDocument<PersonPatchDTO> patchDocument)
        {
            if (patchDocument == null)
            {
                return BadRequest();
            }

            var entityFromDB = await _context.People.FirstOrDefaultAsync(x => x.Id == id);

            if (entityFromDB == null)
            {
                return NotFound();
            }

            var entityDTO = _mapper.Map<PersonPatchDTO>(entityFromDB);

            patchDocument.ApplyTo(entityDTO, ModelState);

            var isValid = TryValidateModel(entityDTO);

            if (!isValid)
            {
                return BadRequest(ModelState);
            }

            _mapper.Map(entityDTO, entityFromDB);

            await _context.SaveChangesAsync();

            return Ok("Modifield success");
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