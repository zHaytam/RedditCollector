using Collector.Properties;
using Microsoft.EntityFrameworkCore;

namespace Collector.Models
{
    public class DatabaseContext : DbContext
    {

        public static DatabaseContext New => new DatabaseContext();

        public DbSet<Subreddit> Subreddits { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(Resources.ConnectionString);
        }

    }
}
