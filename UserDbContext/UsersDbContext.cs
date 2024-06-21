using Microsoft.EntityFrameworkCore;
using SampleAppWithCaching.Models;

namespace SampleAppWithCaching.UserDbContext
{
    public class UsersDbContext : DbContext
    {
        public UsersDbContext(DbContextOptions<UsersDbContext> options) : base(options)
        {

        }
        public DbSet<User> users { get; set; }
    }
}
