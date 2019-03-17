using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using FillThePool.Data;
using FillThePool.Models;
using WebMatrix.WebData;

namespace FillThePool.Web.Controllers.API
{
    [Authorize]
    public class RegistrationApiController : ApiController
    {
        private readonly ApplicationUnit _unit = new ApplicationUnit();

        // GET api/Registration
        public IEnumerable<Registration> GetRegistrations()
        {
            return _unit.Registrations.GetAll().AsEnumerable();
        }

        // GET api/Registration/5
        public Registration GetRegistration(int id)
        {
            var registration = _unit.Registrations.GetById(id);
            if (registration == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return registration;
        }

        // POST api/Registration
        [Authorize]
        public HttpResponseMessage PostRegistration(RegistrationDto registrationDto)
        {
            //make sure the current user has enough credit
            HttpResponseMessage response;
	        var student = _unit.Students.GetById(registrationDto.StudentId);
            var balance = Helpers.CheckBalance(student.User.UserId);

            if (balance > 0)
            {
                var schedule = _unit.Schedules.GetById(registrationDto.ScheduleId);
	            if (schedule.Registration == null)
	            {
		            schedule.Registration = new Registration {TimeStamp = DateTime.Now.AddHours(-7), Student = student};

		            var transaction = new Transaction
		            {
			            Amount = -1,
			            LessonCredit = -1,
			            Description = "Reserved Lesson",
			            User = _unit.Users.GetById(student.User.UserId),
			            Type = "Reserved Lesson",
			            TimeStamp = DateTime.Now
		            };
		            _unit.Transactions.Add(transaction);

		            _unit.SaveChanges();

		            var template = _unit.EmailTemplates.GetAll().FirstOrDefault(x => x.Type == "Schedule");
		            if (template != null)
		            {
			            template.Html = template.Html.Replace("{{student}}", schedule.Registration.Student.Name);
			            template.Html = template.Html.Replace("{{date}}", schedule.Start.ToShortDateString());
			            template.Html = template.Html.Replace("{{time}}", schedule.Start.ToShortTimeString());

			            var user = _unit.Users.GetAll().FirstOrDefault(x => x.UserId == student.User.UserId);
			            if (user != null)
				            Email.SendEmail(user.Email, template.Subject, template.Html);
		            }

		            response = Request.CreateResponse(HttpStatusCode.Created, schedule.Registration);

	            }
	            else
	            {
		            response = Request.CreateResponse(HttpStatusCode.Conflict);
	            }
            }
            else
            {
                response = Request.CreateResponse(HttpStatusCode.PaymentRequired);
            }


            return response;
        }

        // DELETE api/Registration
        public HttpResponseMessage DeleteRegistration(RegistrationDto registrationDto)
        {
            Schedule schedule = _unit.Schedules.GetById(registrationDto.ScheduleId);
	        var userId = schedule.Registration.Student.User.UserId;
            HttpResponseMessage response;
            if (schedule.Start > DateTime.Now.AddHours(-7).AddDays(1))
            {
                //credit the user for the canceled lesson
                var transaction = new Transaction { Amount = -1, LessonCredit = 1, Description = "Canceled Lesson", User = _unit.Users.GetById(WebSecurity.CurrentUserId), Type = "Canceled Lesson", TimeStamp = DateTime.Now.AddHours(-7) };
                _unit.Transactions.Add(transaction);

                _unit.Registrations.Delete(_unit.Registrations.GetById(schedule.Registration.RegistrationId));
                _unit.SaveChanges();
                var template = _unit.EmailTemplates.GetAll().FirstOrDefault(x => x.Type == "Cancellation");
	            if (template != null)
	            {
		            var message = template.Html;
					message = message.Replace("{{date}}", schedule.Start.ToShortDateString());
					message = message.Replace("{{time}}", schedule.Start.ToShortTimeString());

		            var user = _unit.Users.GetAll().FirstOrDefault(x => x.UserId == userId);
		            if (user != null)
			            Email.SendEmail(user.Email, template.Subject, message);
	            }


	            response = Request.CreateResponse(HttpStatusCode.Accepted, schedule.ScheduleId);
                
            }
            else
            {
                response = Request.CreateResponse(HttpStatusCode.Conflict, schedule.Registration.RegistrationId);
            }

            return response;
        }


        protected override void Dispose(bool disposing)
        {
            _unit.Dispose();
            base.Dispose(disposing);
        }
    }


    public class RegistrationDto
    {
	    public RegistrationDto(int studentId, int scheduleId)
	    {
		    ScheduleId = scheduleId;
		    StudentId = studentId;
	    }

	    public int ScheduleId { get; set; }
        public int StudentId { get; set; }
    }

}