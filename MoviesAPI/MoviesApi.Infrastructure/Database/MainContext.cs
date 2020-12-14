using Microsoft.EntityFrameworkCore;
using MoviesApi.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace MoviesApi.Infrastructure.Database
{
   public class MainContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source = MoviesApi.Db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //FluentApi
            modelBuilder.Entity<MoviesGenre>().HasKey(c => new { c.GenreId, c.MovieId });
            modelBuilder.Entity<MoviesActor>().HasKey(x => new { x.PersonId, x.MovieId });
            modelBuilder.Seed();
        }

        public DbSet<Genre> Genres { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<MoviesActor> MoviesActors { get; set; }
        public DbSet<MoviesGenre> MoviesGenres { get; set; }
        public DbSet<Person> People { get; set; }
    }
}
