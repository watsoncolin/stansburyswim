using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using FillThePool.Models;

namespace FillThePool.Data.Configuration
{
    public class CustomDatabaseInitializer : DropCreateDatabaseIfModelChanges<DataContext>
    {
    }
}
