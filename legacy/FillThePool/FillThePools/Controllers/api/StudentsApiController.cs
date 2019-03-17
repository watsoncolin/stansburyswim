using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using FillThePool.Data;
using FillThePool.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using WebMatrix.WebData;

namespace FillThePool.Web.Controllers.API
{
	[Authorize]
	public class StudentsApiController : ApiController
	{
		private readonly ApplicationUnit _unit = new ApplicationUnit();

		public string Get()
		{
			var students = _unit.Students.GetAll().Where(x => x.User.UserId == WebSecurity.CurrentUserId).ToList();

			var settings = new JsonSerializerSettings {ContractResolver = new CamelCasePropertyNamesContractResolver()};

			return JsonConvert.SerializeObject(students, settings);
		}

		// GET api/Schedule/5
		[HttpGet]
		public Student Get(int id)
		{
			var student = _unit.Students.GetById(id);

			if (!User.IsInRole("Admin") && student.User.UserId != WebSecurity.CurrentUserId)
				student = null;				

			if (student == null)
			{
				throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
			}

			return student;
		}

		// PUT api/Schedule/5
		public HttpResponseMessage Put(int id, Student student)
		{
			if (!ModelState.IsValid)
			{
				return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
			}

			if (id != student.StudentId)
			{
				return Request.CreateResponse(HttpStatusCode.BadRequest);
			}

			//Student existingStudent= _unit.Students.GetById(id);

			if (!User.IsInRole("Admin") && student.User.UserId != WebSecurity.CurrentUserId)
				return Request.CreateResponse(HttpStatusCode.NotFound);

			//_unit.Students.Detatch(existingStudent);
			//student.CreatedOn = existingStudent.CreatedOn;
			_unit.Students.Update(student);

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
		public HttpResponseMessage Post(Student student)
		{
			try
			{
				if (ModelState.IsValid)
				{
					student.User = _unit.Users.GetById(WebSecurity.CurrentUserId);
					_unit.Students.Add(student);
					_unit.SaveChanges();

					var response = Request.CreateResponse(HttpStatusCode.Created, student);

					response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = student.StudentId }));

					return response;
				}
				return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
			}
			catch (Exception ex)
			{
				return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
			}
		}

		[HttpDelete]
		public HttpResponseMessage Delete(int id)
		{
			Student student = _unit.Students.GetById(id);

			if (student != null)
				if (!User.IsInRole("Admin") && student.User.UserId != WebSecurity.CurrentUserId)
					student = null;

			if (student == null)
			{
				return Request.CreateResponse(HttpStatusCode.NotFound);
			}

			//cancel all lessons >24hr and refund to the user
			List<Schedule> schedules = _unit.Schedules.GetAll().Where(x => x.Registration.Student.User.UserId == WebSecurity.CurrentUserId).ToList();
			
			foreach (Schedule s in schedules)
			{
				if (s.Start > DateTime.Now.AddHours(24))
				{
					var transaction = new Transaction { Amount = -1, LessonCredit = 1, Description = "Registration Canceled", User = s.Registration.Student.User, Type = "Canceled", TimeStamp = DateTime.Now.AddHours(-7) };
					_unit.Transactions.Add(transaction);
					var template = _unit.EmailTemplates.GetAll().FirstOrDefault(x => x.Type == "Cancellation");
					if (template != null)
					{
						var message = template.Html;
						message = message.Replace("{{date}}", s.Start.ToShortDateString());
						message = message.Replace("{{time}}", s.Start.ToShortTimeString());

						Email.SendEmail(s.Registration.Student.User.Email, template.Subject, message);
					}
				}

				_unit.Registrations.Delete(_unit.Registrations.GetById(s.Registration.RegistrationId));			
			}

			_unit.Students.Delete(student);

			try
			{
				_unit.SaveChanges();
				return Request.CreateResponse(HttpStatusCode.OK, student);
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