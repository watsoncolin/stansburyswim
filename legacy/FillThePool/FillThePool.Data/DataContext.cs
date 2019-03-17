using System;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using FillThePool.Data.Configuration;
using FillThePool.Models;

namespace FillThePool.Data
{
    public class DataContext : DbContext
    {        
        public DbSet<User> Users { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Registration> Registrations { get; set; }
		public DbSet<Schedule> Schedules { get; set; }
		public DbSet<PromoCode> PromoCode { get; set; }
		public DbSet<EmailTemplate> EmailTemplates { get; set; }
		public DbSet<Setting> Settings { get; set; }
        public DbSet<Role> Roles { get; set; }
		public DbSet<Page> Pages { get; set; }
		public DbSet<WaitList> WaitList { get; set; }

	    private static string ConnectionStringName
        {
            get
            {
	            return ConfigurationManager.AppSettings["ConnectionStringName"] ?? "DefaultConnection";
            }
        }

        static DataContext()
        {
			//
            Database.SetInitializer(new CustomDatabaseInitializer());
        }

        public DataContext() : base(ConnectionStringName) { }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new ScheduleConfiguration());
            modelBuilder.Configurations.Add(new UserConfiguration());

            // Add simple memberhsip tables
            modelBuilder.Configurations.Add(new MembershipConfiguration());
            modelBuilder.Configurations.Add(new RoleConfiguration());
            modelBuilder.Configurations.Add(new OAuthMembershipConfiguration());

            //base.OnModelCreating(modelBuilder);
        }

        private void ApplyRules()
        {
            foreach (var entry in ChangeTracker.Entries().Where(e => e.Entity is IAuditInfo && ((e.State == EntityState.Added) || (e.State == EntityState.Modified))))
            {
                var e = (IAuditInfo)entry.Entity;

                if (entry.State == EntityState.Added)
                    e.CreatedOn = DateTime.Now;

                e.ModifiedOn = DateTime.Now;
            }
        }

        public override int SaveChanges()
        {
            ApplyRules();
            return base.SaveChanges();
        }
    }
}
