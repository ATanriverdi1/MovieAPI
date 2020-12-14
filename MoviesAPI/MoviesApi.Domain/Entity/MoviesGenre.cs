using System;
using System.Collections.Generic;
using System.Text;

namespace MoviesApi.Domain.Entity
{
   public class MoviesGenre
    {
        public int MovieId { get; set; }
        public int GenreId { get; set; }

        public Genre Genre { get; set; }
        public Movie Movie { get; set; }
    }
}
