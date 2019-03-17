using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FillThePool.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace FillThePool.Web.ViewModels
{
	public class AccountViewModel : ViewModelBase
	{
		public User User { get; set; }
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

		public LocalPasswordModel LocalPasswordModel { get; set; }
		
		public Student Student { get; set; }
		public IList<Student> Students { get; set; }
		

		public AccountViewModel()
		{
			Student = new Student();
			User = new User();
			LocalPasswordModel = new LocalPasswordModel();
		}

		public string StudentsJson
		{
			get
			{
				var settings = new JsonSerializerSettings {ContractResolver = new CamelCasePropertyNamesContractResolver()};

				var students = JsonConvert.SerializeObject(Students, settings);
				return students;
			}
		}
	}
}