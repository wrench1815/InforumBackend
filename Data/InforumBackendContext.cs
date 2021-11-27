using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using InforumBackend.Models;

namespace InforumBackend.Data
{
    public class InforumBackendContext : DbContext
    {
        public InforumBackendContext(DbContextOptions<InforumBackendContext> options)
            : base(options)
        {
        }

        public DbSet<Answer> Answer { get; set; }

        public DbSet<BlogPost> BlogPost { get; set; }

        public DbSet<Category> Category { get; set; }

        public DbSet<Comment> Comment { get; set; }

        public DbSet<ContactForm> ContactForm { get; set; }

        public DbSet<Home> Home { get; set; }

        public DbSet<Query> Query { get; set; }
    }
}
