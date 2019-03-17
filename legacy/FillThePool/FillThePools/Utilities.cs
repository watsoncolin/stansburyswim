using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using FillThePool.Data;
using FillThePool.Models;

namespace FillThePool.Web
{
	public class Utilities
	{

		ApplicationUnit unit = new ApplicationUnit();

		public static string ProcessTemplate(string template)
		{
			var placeholders = new List<string>();
			


			return "";
		}


		public void SetupLessons()
		{
			var startDate = DateTime.Parse(ConfigurationManager.AppSettings["LessonStartDate"]);
			var endDate = DateTime.Parse(ConfigurationManager.AppSettings["LessonEndDate"]);
			var schedules = unit.Schedules.GetAll();
			var startTime = DateTime.Parse(ConfigurationManager.AppSettings["LessonStartTime"]);
			var endTime = DateTime.Parse(ConfigurationManager.AppSettings["LessonEndTime"]);
			var duration = Int32.Parse(ConfigurationManager.AppSettings["LessonDuration"]);

			//foreach day between the start and end dates make sure there is at least lessons between the start and end times
			// if the lessons are filled already open two time slots around the first lesson and last lesson of the day until we've reached the earliest and latest times


			var currentDate = startDate;
			while (currentDate < endDate)
			{
				var currentTime = startTime;

				while (currentTime < endDate)
				{
					//make sure current time exists
					if (!schedules.Any(s => s.Start == currentDate && s.Start.TimeOfDay == currentTime.TimeOfDay && s.Lane == "Lane 1"))
					{
						var newLesson = currentDate.AddMilliseconds(currentTime.Millisecond);
						unit.Schedules.Add(new Schedule { CreatedOn = DateTime.Now, End = newLesson.AddMinutes(duration), Lane = "Lane 1", ModifiedOn = DateTime.Now, Start = newLesson });
					}	
					
					//make sure current time exists
					if (!schedules.Any(s => s.Start.Date == currentDate && s.Start.TimeOfDay == currentTime.TimeOfDay && s.Lane == "Lane 2"))
					{
						var newLesson = currentDate.AddMilliseconds(currentTime.Millisecond);
						unit.Schedules.Add(new Schedule { CreatedOn = DateTime.Now, End = newLesson.AddMinutes(duration), Lane = "Lane 2", ModifiedOn = DateTime.Now, Start = newLesson });
					}

					//check if we need to expand the schedule
					ExpandSchedule(currentDate.AddMilliseconds(currentTime.Millisecond), "Lane 1", null);
					ExpandSchedule(currentDate.AddMilliseconds(currentTime.Millisecond), "Lane 2", null);

					currentTime = currentTime.AddMinutes(duration);
				}

				currentDate = currentDate.AddDays(1);
			}

		}
		public void ExpandSchedule(DateTime lessonDateTime, string lane, bool? reverse)
		{
			var startDate = DateTime.Parse(ConfigurationManager.AppSettings["LessonStartDate"]);
			var endDate = DateTime.Parse(ConfigurationManager.AppSettings["LessonEndDate"]);
			var duration = Int32.Parse(ConfigurationManager.AppSettings["LessonDuration"]);
			var startTime = DateTime.Parse(ConfigurationManager.AppSettings["LessonStartTime"]);
			var endTime = DateTime.Parse(ConfigurationManager.AppSettings["LessonEndTime"]);
			//if registered expand schedule with in parameters if not registered continue.

			if (unit.Schedules.GetAll().Any(s => s.Start == lessonDateTime && s.Registration != null && s.Lane == lane))
			{
				if (reverse != null && (bool) !reverse)
				{
					if (lessonDateTime.AddMinutes(duration).TimeOfDay < endTime.TimeOfDay)
					{
						ExpandSchedule(lessonDateTime.AddMinutes(duration), lane, false);
					}
				}
				else if (reverse != null)
				{
					if (lessonDateTime.AddMinutes(duration * -1).TimeOfDay < startTime.TimeOfDay)
					{
						ExpandSchedule(lessonDateTime.AddMinutes(duration), lane, true);
					}
				}
				else
				{
					ExpandSchedule(lessonDateTime.AddMinutes(duration), lane, true);
					ExpandSchedule(lessonDateTime.AddMinutes(duration), lane, false);
				}
			}

			if (!unit.Schedules.GetAll().Any(s => s.Start == lessonDateTime && s.Lane == lane))
			{
				if (lessonDateTime.TimeOfDay > startTime.TimeOfDay && lessonDateTime.TimeOfDay < endTime.TimeOfDay &&
				    lessonDateTime.Date > startDate.Date && lessonDateTime.Date < endDate.Date && lessonDateTime.DayOfWeek != DayOfWeek.Sunday && lessonDateTime.DayOfWeek != DayOfWeek.Saturday)
				{
					unit.Schedules.Add(new Schedule
					{
						CreatedOn = DateTime.Now,
						ModifiedOn = DateTime.Now,
						Start = lessonDateTime,
						End = lessonDateTime.AddMinutes(duration),
						Lane = lane
					});
				}
			}

		}
	}
}