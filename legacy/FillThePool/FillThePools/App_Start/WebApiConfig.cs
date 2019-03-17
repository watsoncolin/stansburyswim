using System.Web.Http;

namespace FillThePool.Web
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
			config.Routes.MapHttpRoute(
				name: "ScheduleAPI",
				routeTemplate: "api/schedule/{id}",
				defaults: new { controller = "SchedulesApi", id = RouteParameter.Optional }
				);

			config.Routes.MapHttpRoute(
				name: "StudentAPI",
				routeTemplate: "api/student/{id}",
				defaults: new { controller = "StudentsApi", id = RouteParameter.Optional }
				);

			config.Routes.MapHttpRoute(
				name: "UserAPI",
				routeTemplate: "api/user/{id}",
				defaults: new { controller = "UserApi" }
				);

			config.Routes.MapHttpRoute(
				name: "RegistrationApi",
				routeTemplate: "api/registration/{id}",
				defaults: new { controller = "RegistrationApi", id = RouteParameter.Optional }
				);

	        config.Routes.MapHttpRoute(
		        name: "PageApi",
				routeTemplate: "api/Page/{id}",
				defaults: new { controller = "PageApi", id = RouteParameter.Optional }
				);

	        config.Routes.MapHttpRoute(
				name: "SMSDelivery",
				routeTemplate: "api/sms/{action}",
				defaults: new { controller = "SmsDeliveryApi" }
		        );

			config.Routes.MapHttpRoute(
				name: "Error",
				routeTemplate: "api/error/",
				defaults: new { controller = "Error" });

	        config.Routes.MapHttpRoute(
		        name: "WaitListApi",
		        routeTemplate: "api/waitlist/update/{onWaitList}",
		        defaults: new {controller = "WaitlistAPI"});

			config.Routes.MapHttpRoute(
				name: "WaitListApi2",
				routeTemplate: "api/waitlist/email/{id}",
				defaults: new { controller = "WaitlistAPI" });

			config.Routes.MapHttpRoute(
				name: "ApiByAction",
				routeTemplate: "api/{controller}/{action}",
				defaults: new { action = "Get" }
			);


			
			config.Routes.MapHttpRoute(
				name: "DefaultApi",
				routeTemplate: "api/{controller}/{id}",
				defaults: new { id = RouteParameter.Optional} 
				);


		//	config.Routes.MapHttpRoute(
		//	name: "ApiById",
		//	routeTemplate: "api/{controller}/{id}",
		//	defaults: new { id = RouteParameter.Optional },
		//	constraints: new { id = @"^[0-9]+$" }
		//);

		//	config.Routes.MapHttpRoute(
		//		name: "ApiByName",
		//		routeTemplate: "api/{controller}/{action}/{name}",
		//		defaults: null,
		//		constraints: new { name = @"^[a-z]+$" }

            // Uncomment the following line of code to enable query support for actions with an IQueryable or IQueryable<T> return type.
            // To avoid processing unexpected or malicious queries, use the validation settings on QueryableAttribute to validate incoming queries.
            // For more information, visit http://go.microsoft.com/fwlink/?LinkId=279712.
            //config.EnableQuerySupport();
        }
    }
}