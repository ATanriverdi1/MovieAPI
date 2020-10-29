using Microsoft.AspNetCore.Http;
using System;

namespace MoviesAPI.DTOs.Person
{
    public class PersonCreationDTO : PersonPatchDTO
    {
        public IFormFile Picture { get; set; }
    }
}
