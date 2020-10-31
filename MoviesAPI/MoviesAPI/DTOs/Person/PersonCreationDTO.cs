using Microsoft.AspNetCore.Http;

namespace MoviesAPI.DTOs.Person
{
    public class PersonCreationDTO : PersonPatchDTO
    {
        public IFormFile Picture { get; set; }
    }
}