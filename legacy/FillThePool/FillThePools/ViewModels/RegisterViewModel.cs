using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FillThePool.Web.ViewModels
{
	public class RegisterViewModel
	{
		[Required]
		[Display(Name = "Username")]
		public string UserName { get; set; }

		[Required]
		[StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
		[DataType(DataType.Password)]
		[Display(Name = "Password")]
		public string Password { get; set; }

		[DataType(DataType.Password)]
		[Display(Name = "Confirm Password")]
		[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
		public string ConfirmPassword { get; set; }
		[Required]
		public string Email { get; set; }
		public string Phone { get; set; }
		public string Address { get; set; }
		public string Address2 { get; set; }
		public string City { get; set; }
		public string State { get; set; }
		public string Zip { get; set; }
		[Display(Name = "First Name")]
		public string FirstName { get; set; }
		[Display(Name = "Last Name")]
		public string LastName { get; set; }
		[Display(Name = "Mobile Phone")]
		public string MobilePhone { get; set; }
		[Display(Name = "Carrier")]
		public string MobileCarrier { get; set; }
		public Dictionary<string, string> CarrierOptions {
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
		[Display(Name = "Private Registration (hide names on public lesson schedule):  ")]
		public bool PrivateRegistration { get; set; }

		[Display(Name = "How did you hear about us?")]
		public string Referral { get; set; }
	}
}