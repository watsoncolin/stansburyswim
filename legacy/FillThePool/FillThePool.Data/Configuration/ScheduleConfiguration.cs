using FillThePool.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FillThePool.Data.Configuration
{
    public class ScheduleConfiguration : EntityTypeConfiguration<Schedule>
    {
        public ScheduleConfiguration()
        {
            this.Property(p => p.End).IsRequired();
            this.Property(p => p.Start).IsRequired();
        }
    }
}
