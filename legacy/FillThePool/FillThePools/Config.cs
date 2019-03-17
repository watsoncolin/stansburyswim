using System.Configuration;
using System.Linq;
using FillThePool.Data;

namespace FillThePool.Web
{
	public static class Config
	{
		public static string ConnectionStringName
		{
			get { return ConfigurationManager.AppSettings["ConnectionStringName"] ?? "DefaultConnection"; }
		}

		public static string UsersTableName
		{
			get { return ConfigurationManager.AppSettings["UsersTableName"] ?? "Users"; }
		}

		public static string UsersPrimaryKeyColumnName
		{
			get { return ConfigurationManager.AppSettings["UsersPrimaryKeyColumnName"] ?? "UserId"; }
		}

		public static string UsersUserNameColumnName
		{
			get { return ConfigurationManager.AppSettings["UsersUserNameColumnName"] ?? "Username"; }
		}

		public static bool PayPalLive
		{
			get
			{
				var live = ConfigurationManager.AppSettings["PayPalLive"] != null &&
				            ConfigurationManager.AppSettings["PayPalLive"] == "true";

				return live;
			}
		}

		public static string PayPalAppId
		{
			get { return ConfigurationManager.AppSettings["PayPalAppId"] ?? ""; }
		}

		public static string PayPalAppSecret
		{
			get { return ConfigurationManager.AppSettings["PayPalAppSecret"] ?? ""; }
		}

		public static string SendGridUser
		{
			get { return ConfigurationManager.AppSettings["SendGridUser"] ?? ""; }
		}

		public static string SendGridPassword
		{
			get { return ConfigurationManager.AppSettings["SendGridPass"] ?? ""; }
		}

		public static string SendFrom
		{
			get
			{
				var unit = new ApplicationUnit();
				var setting = unit.Settings.GetAll().FirstOrDefault(x => x.Key == "DefaultFrom");
				return setting != null ? setting.Value : "";
			}
		}

		public static string SiteName
		{
			get
			{
				var unit = new ApplicationUnit();
				var setting = unit.Settings.GetAll().FirstOrDefault(x => x.Key == "SiteName");
				return setting != null ? setting.Value : "";
			}
		}
	}
}