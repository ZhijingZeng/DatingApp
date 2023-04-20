using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<AppUser> Users {get; set;}
        public DbSet<UserLike> Likes {get; set;}

        protected override void OnModelCreating (ModelBuilder builder){
            base.OnModelCreating(builder);
            builder.Entity<UserLike>()//<UserLike> entity we target here relationship
                .HasKey(k =>new {k.SourceUserId, k.TargetUserId}); //primary key
            builder.Entity<UserLike>()
                .HasOne(s => s.SourceUser) //AppUser table
                .WithMany(l =>l.LikedUsers) //Likes table
                .HasForeignKey(s=>s.SourceUserId) //how to connect
                .OnDelete(DeleteBehavior.Cascade); //delete a appuser will delete all its liked users
            builder.Entity<UserLike>()
                .HasOne(s => s.TargetUser)
                .WithMany(l =>l.LikedByUsers)
                .HasForeignKey(s=>s.TargetUserId)
                .OnDelete(DeleteBehavior.Cascade);
            
        }
    }
}



// There are 2 ways of applying migrations and they both do the same thing - create the DB if it does not exist and apply any migrations that have not been already applied to the DB:

// 1.  Using the dotnet ef database update command

// 2.  Using the context.Database.Migrate() in the code.

// We are using option 2 in the Program.cs class so we do not need to update the DB using the command line. 

