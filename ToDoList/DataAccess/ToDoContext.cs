using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ToDoList.Models;

namespace ToDoList.DataAccess
{
    public class ToDoContext : DbContext
    {
        public ToDoContext(DbContextOptions options) : base(options) { }
        public DbSet<ToDoModel> ToDoSet { get; set; }
        public DbSet<ToDoListUser> ToDoListUser { get; set; }
        public DbSet<IdentityUserLogin<string>> User { get; set; }
        public DbSet<IdentityUserClaim<string>> UserClaim { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("User").HasKey(t => t.UserId);
            modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("UClaims").HasKey(t => t.Id);
            modelBuilder.Entity<ToDoModel>().ToTable("ToDos").HasKey(t => t.Id);
            base.OnModelCreating(modelBuilder);
        }
    }
}
