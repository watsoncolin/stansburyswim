using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FillThePool.Core.Data
{
	public class ApplicationDbContext : IdentityDbContext
	{
		public DbSet<Profile> Profiles { get; set; }
		public DbSet<Student> Students { get; set; }
		public DbSet<Product> Products { get; set; }
		public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Pool> Pools { get; set; }
		public DbSet<Instructor> Instructors { get; set; }
		public DbSet<Schedule> Schedules { get; set; }
		public DbSet<Registration> Registrations { get; set; }
		public DbSet<EmailTemplate> EmailTemplates { get; set; }

		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options)
		{

		}

		protected override void OnModelCreating(ModelBuilder builder)
		{
			builder.Entity<Profile>()
				.HasMany(p => p.Students)
				.WithOne(s => s.Profile);

			base.OnModelCreating(builder);
		}
	}

	public class Instructor
	{
		public int Id { get; set; }
		public bool Active { get; set; }
		public string Name { get; set; }
		public string Bio { get; set; }
		public string Image { get; set; }
	}
}
