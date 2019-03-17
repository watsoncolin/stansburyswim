using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FillThePool.Models
{
    [Table("UserProfile")]
    public class UserProfile
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
		public int UserId { get; set; }
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
				var dictionary = new Dictionary<string, string>
				{
					{"AT&T", "AT&T"},
					{"T-Mobile", "T-Mobile"},
					{"Verizon", "Verizon"},
					{"Other", "Other"}
				};

				return dictionary;
			}
		}
		[Display(Name = "Private Reservations")]
		public bool PrivateRegistration { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
    }

    public class RegisterExternalLoginModel
    {
        [Required]
        [Display(Name = "Username")]
        public string UserName { get; set; }
		[Required]
		[Display(Name = "First Name")]
		public string FirstName { get; set; }
		[Display(Name = "Last Name")]
		public string LastName { get; set; }
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
				var dictionary = new Dictionary<string, string>
				{
					{"AT&T", "AT&T"},
					{"T-Mobile", "T-Mobile"},
					{"Verizon", "Verizon"},
					{"Other", "Other"}
				};

				return dictionary;
			}
		}
	    [Display(Name = "Private Reservations")]
		public bool PrivateRegistration { get; set; }


		[Display(Name = "How did you hear about us?")]
		public string Referral { get; set; }
        public string ExternalLoginData { get; set; }
    }

    public class LocalPasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class LoginModel
    {
        [Required]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

	public class ExternalLogin
    {
        public string Provider { get; set; }
        public string ProviderDisplayName { get; set; }
        public string ProviderUserId { get; set; }
    }
}
