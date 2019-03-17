using System.Collections.Generic;
using FillThePool.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace FillThePool.Web.ViewModels
{
    public class ScheduleListViewModel : ViewModelBase
    {
        public IList<Schedule> Schedules { get; set; }
		public IList<Registration> Registrations { get; set; }
		public IList<Student> Students { get; set; }
        
        public string SchedulesJson
        {
            get
            {
                var settings = new JsonSerializerSettings
                {
	                ContractResolver = new CamelCasePropertyNamesContractResolver()
                };

	            var schedules = JsonConvert.SerializeObject(Schedules, settings);
                return schedules;
            }
        }

		public string RegistrationsJson
		{
			get
			{
				var settings = new JsonSerializerSettings {ContractResolver = new CamelCasePropertyNamesContractResolver()};

				var registrations = JsonConvert.SerializeObject(Registrations, settings);
				return registrations;
			}
		}

		public string StudentsJson
		{
			get
			{
				var settings = new JsonSerializerSettings {ContractResolver = new CamelCasePropertyNamesContractResolver()};

				var students = JsonConvert.SerializeObject(this.Students, settings);
				return students;
			}
		}
    }
}