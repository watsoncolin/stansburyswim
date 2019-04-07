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
}
