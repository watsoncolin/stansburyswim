using System.Web.Http;
using System.Web.Http.Validation.Providers;
using FillThePool.Web.Filters;
using Newtonsoft.Json.Serialization;

namespace FillThePool.Web
{
    public static class CustomGlobalConfig
    {
        public static void Customize(HttpConfiguration config)
        {
            config.Services.RemoveAll(typeof(System.Web.Http.Validation.ModelValidatorProvider), v => v is InvalidModelValidatorProvider);
            config.Formatters.Remove(config.Formatters.XmlFormatter);
            var json = config.Formatters.JsonFormatter;
            json.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            config.Filters.Add(new ValidationActionFilter());
			config.Filters.Add(new NexmoActionFilter());
        }
    }
}