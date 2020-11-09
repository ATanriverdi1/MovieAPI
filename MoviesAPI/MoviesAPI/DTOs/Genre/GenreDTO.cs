using System.Collections.Generic;

namespace MoviesAPI.DTOs
{
    public class GenreDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Link> Links { get; set; } = new List<Link>();
    }
}