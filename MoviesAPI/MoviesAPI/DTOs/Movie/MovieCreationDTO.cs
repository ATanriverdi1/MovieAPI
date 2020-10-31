using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesAPI.DTOs.Movie
{
    public class MovieCreationDTO : MoviePatchDTO
    {
        public IFormFile Poster { get; set; }
    }
}
