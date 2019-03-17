using System.Collections.Generic;
using FillThePool.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace FillThePool.Web.ViewModels
{
	public class StudentListViewModel : ViewModelBase
	{
		public Student Student { get; set; }
		public IList<Student> Students { get; set; }
		
		public StudentListViewModel()
		{
			Student = new Student();
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