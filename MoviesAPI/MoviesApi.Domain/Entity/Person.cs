using System;
using System.Collections.Generic;
using System.Text;

namespace MoviesApi.Domain.Entity
{
   public class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Biography { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Picture { get; set; }

        public List<MoviesActor> MoviesActors { get; set; }
    }
}
