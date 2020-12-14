using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MoviesApi.Domain.Entity
{
   public class Genre
    {
        public int Id { get; set; }

        [Required]
        [StringLength(40)]
        public string Name { get; set; }

        public List<MoviesGenre> MoviesGenres { get; set; }
    }
}
