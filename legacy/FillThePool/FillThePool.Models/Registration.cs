using System;

namespace FillThePool.Models
{
    public class Registration : IAuditInfo
    {
        public int RegistrationId { get; set; }
        public DateTime TimeStamp { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }

        virtual public Student Student { get; set; }
    }
}