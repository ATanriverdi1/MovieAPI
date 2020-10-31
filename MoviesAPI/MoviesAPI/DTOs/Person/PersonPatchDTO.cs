using System;

namespace MoviesAPI.DTOs.Person
{
    public class PersonPatchDTO
    {
        public string Name { get; set; }
        public string Biography { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
}