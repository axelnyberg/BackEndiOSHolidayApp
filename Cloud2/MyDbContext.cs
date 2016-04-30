using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace Cloud2
{
    public class MyDbContext : IdentityDbContext<IdentityUser>
    {

        public MyDbContext()
            : base("MyDbContext")
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Vacation> Vacations { get; set; }
        public DbSet<Memory> Memories { get; set; }
        public DbSet<Media> Medias { get; set; }



        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>().HasMany(u => u.friendList).WithMany();
        }
    }
    
}