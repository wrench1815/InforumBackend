using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using InforumBackend.Models;
using InforumBackend.Authentication;
namespace InforumBackend.Data
{
    public class InforumBackendContext : IdentityDbContext<ApplicationUser>
    {
        public InforumBackendContext(DbContextOptions<InforumBackendContext> options)
            : base(options)
        {
        }

        public DbSet<BlogPost> BlogPost { get; set; }

        public DbSet<Category> Category { get; set; }

        public DbSet<Comment> Comment { get; set; }

        public DbSet<ContactForm> ContactForm { get; set; }

        public DbSet<FirstRun> FirstRun { get; set; }

        public DbSet<Home> Home { get; set; }

        public DbSet<ForumAnswer> ForumAnswer { get; set; }

        public DbSet<ForumQuery> ForumQuery { get; set; }

        public DbSet<ForumSubAnswer> ForumSubAnswer { get; set; }

        public DbSet<Star> Star { get; set; }

        public DbSet<SubComment> SubComment { get; set; }

        public DbSet<Vote> Vote { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
