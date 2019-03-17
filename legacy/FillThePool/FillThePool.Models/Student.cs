using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace FillThePool.Models
{
    public class Student : IAuditInfo
    {
        public int StudentId { get; set; }
        public string Name { get; set; }
        public DateTime BirthDate { get; set; }
        public string Ability { get; set; }
        public string Notes { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }

        virtual public int RegistrationId { get; set; }
        [JsonIgnore] 
        virtual public List<Registration> Registrations { get; set; }
        virtual public User User { get; set; }
    }
}