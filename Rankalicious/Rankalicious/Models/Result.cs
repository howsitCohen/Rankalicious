using System;
using System.Data.Entity;

namespace Rankalicious.Models
{
    public class Result
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string Genre { get; set; }
        public decimal Price { get; set; }
    }

    public class ResultsDbContext : DbContext
    {
        public DbSet<Result> Results { get; set;}
    }
}