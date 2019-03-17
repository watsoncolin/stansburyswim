using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using FillThePool.Data;
using FillThePool.Models;

namespace FillThePool.Web.Controllers.API
{
	[Authorize]
    public class SchedulesApiController : ApiController
    {
        private readonly ApplicationUnit _unit = new ApplicationUnit();

        // GET api/Schedule
		[HttpGet]
        [AllowAnonymous]
		public IEnumerable<Schedule> GetSchedules()
        {
			return _unit.Schedules.GetAll();
        }

        // GET api/Schedule/5
		[Authorize(Roles="Admin")]
        public Schedule GetSchedule(int id)
        {
			Schedule schedule = _unit.Schedules.GetById(id);
            if (schedule == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return schedule;
        }

		// PUT api/Schedule/5
		[Authorize(Roles = "Admin")]
        public HttpResponseMessage PutSchedule(int id, Schedule schedule)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            if (id != schedule.ScheduleId)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

			Schedule existingSchedule = _unit.Schedules.GetById(id);
			_unit.Schedules.Detatch(existingSchedule);
			schedule.CreatedOn = existingSchedule.CreatedOn;

			_unit.Schedules.Update(schedule);

            try
            {
                _unit.SaveChanges();

				return Request.CreateResponse(HttpStatusCode.OK, "{success: 'true', verb: 'PUT'}");
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }
        }

        // POST api/Schedule
        [Authorize(Roles = "Admin")]
        public HttpResponseMessage PostSchedule(Schedule schedule)
        {
			try
			{
				if (ModelState.IsValid)
				{
					_unit.Schedules.Add(schedule);
					_unit.SaveChanges();

					HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, schedule);

					response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = schedule.ScheduleId }));

					return response;
				}
				return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
			}
			catch (Exception ex)
			{
				return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
			}
        }

        [Authorize(Roles = "Admin")]
        public HttpResponseMessage Delete(int id)
        {
			Schedule schedule = _unit.Schedules.GetById(id);

            if (schedule == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }				

            // If there is a registration notify the user and refund.
            if (schedule.Registration != null)
            {
                var transaction = new Transaction { Amount = -1, LessonCredit = 1, Description = "Registration Canceled", User = schedule.Registration.Student.User, Type = "Canceled", TimeStamp = DateTime.Now.AddHours(-7) };
                _unit.Transactions.Add(transaction);
                var template = _unit.EmailTemplates.GetAll().FirstOrDefault(x => x.Type == "Cancellation");

				if (template != null)
				{
					var message = template.Html;
					message = message.Replace("{{date}}", schedule.Start.ToShortDateString());
					message = message.Replace("{{time}}", schedule.Start.ToShortTimeString());

					Email.SendEmail(schedule.Registration.Student.User.Email, template.Subject, message);
				}

	            _unit.Registrations.Delete(_unit.Registrations.GetById(schedule.Registration.RegistrationId));
            }

			_unit.Schedules.Delete(schedule);

            try
            {
                _unit.SaveChanges();
				return Request.CreateResponse(HttpStatusCode.OK, schedule);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }            
		}

        protected override void Dispose(bool disposing)
        {
            _unit.Dispose();
            base.Dispose(disposing);
        }
    }
}