using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using InforumBackend.Models;

namespace InforumBackend.Data {
	public class InforumBackendContext : DbContext {
		public InforumBackendContext(DbContextOptions<InforumBackendContext> options)
			: base(options) {
		}

		public DbSet<InforumBackend.Models.Home> Home { get; set; }

		public DbSet<InforumBackend.Models.ContactForm> ContactForm { get; set; }
	}
}
