using System;
using System.Collections.Generic;
using System.Text;

namespace MoviesApi.Domain.Entity
{
   public class MoviesActor
    {
        public int PersonId { get; set; }
        public int MovieId { get; set; }
        public Movie Movie { get; set; }
        public Person Person { get; set; }
        public string Character { get; set; }
        public int Order { get; set; }
    }
}
