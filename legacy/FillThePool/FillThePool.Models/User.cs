using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.AccessControl;

namespace FillThePool.Models
{
    public class User : IAuditInfo
    {
        public int UserId { get; set; }
		[Display(Name = "First Name")]
		public string FirstName { get; set; }
		[Display(Name = "Last Name")]
        public string LastName { get; set; }
		[Display(Name = "Username")]
        public string UserName { get; set; }
		public string Email { get; set; }
		public string Phone { get; set; }
		public string Address { get; set; }
		public string Address2 { get; set; }
		public string City { get; set; }
		public string State { get; set; }
		public string Zip { get; set; }
		[Display(Name = "Mobile Phone")]
		public string MobilePhone { get; set; }
		[Display(Name = "Carrier")]
		public string MobileCarrier { get; set; }
		public Dictionary<string, string> CarrierOptions
		{
			get
			{
				return new Dictionary<string, string>
				{
					{"AT&T", "AT&T"},
					{"T-Mobile", "T-Mobile"},
					{"Verizon", "Verizon"},
					{"Other", "Other"}
				};
			}
		}
		[Display(Name = "Private Reservations")]
		public bool PrivateRegistration { get; set; }
	    public string Referral { get; set; }
		public DateTime CreatedOn { get; set; }
		public DateTime ModifiedOn { get; set; }

        public ICollection<Role> Roles { get; set; }

        public User()
        {
            this.Roles = new List<Role>();
        }
	}
}
