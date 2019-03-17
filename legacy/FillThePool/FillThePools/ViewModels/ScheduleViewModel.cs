using System;
using FillThePool.Models;
using FillThePool.Data;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FillThePool.Web.ViewModels
{
    public class ScheduleViewModel : ViewModelBase
    {
        public int ScheduleId { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        public Registration Registration { get; set; }
        public Student Student { get; set; }



		public List<DateTime> AvailableDates
		{
			get
			{
				ApplicationUnit unit = new ApplicationUnit();
                // get dates that are full
                var schedules = unit.Schedules.GetAll().Include(s => s.Registration).ToList();
                var dates = new List<DateTime>();
                foreach (var schedule in schedules)
                {
					if (schedules.Any(s => s.Start.DayOfYear == schedule.Start.DayOfYear && s.Registration == null))
					{
						if (!dates.Contains(schedule.Start.Date))
							dates.Add(schedule.Start.Date);
					}
                }
                return dates;
			}
		}

		public string AvailableDatesJSON
		{
			get
			{
				return JsonConvert.SerializeObject(AvailableDates, new JavaScriptDateTimeConverter());
			}
		}
    }
}