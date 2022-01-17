﻿using System;
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

        public DbSet<Answer> Answer { get; set; }

        public DbSet<BlogPost> BlogPost { get; set; }

        public DbSet<Category> Category { get; set; }

        public DbSet<Comment> Comment { get; set; }

        public DbSet<ContactForm> ContactForm { get; set; }

        public DbSet<Home> Home { get; set; }

        public DbSet<ForumQuery> ForumQuery { get; set; }

        public DbSet<SubComment> SubComment { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
