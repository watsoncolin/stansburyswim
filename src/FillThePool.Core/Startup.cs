using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FillThePool.Core.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using Serilog.Exceptions;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace FillThePool.Core
{
	public class Startup
	{
		public Startup(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
		{
			Configuration = configuration;
			var builder = new ConfigurationBuilder()
			   .SetBasePath(hostingEnvironment.ContentRootPath)
			   .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
			   .AddJsonFile($"appsettings.{hostingEnvironment.EnvironmentName}.json", reloadOnChange: true, optional: true)
			   .AddEnvironmentVariables();

			builder.AddUserSecrets<Startup>();

			Configuration = builder.Build();

			var elasticUri = Configuration["ElasticConfiguration:Uri"];

			Log.Logger = new LoggerConfiguration()
			   .Enrich.FromLogContext()
			   .Enrich.WithExceptionDetails()
			   .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(elasticUri))
			   {
				   AutoRegisterTemplate = true,
			   })
		   .CreateLogger();
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.Configure<CookiePolicyOptions>(options =>
			{
				// This lambda determines whether user consent for non-essential cookies is needed for a given request.
				options.CheckConsentNeeded = context => true;
				options.MinimumSameSitePolicy = SameSiteMode.None;
			});

			services.AddDbContext<ApplicationDbContext>(options =>
				options.UseSqlServer(
					Configuration.GetConnectionString("DefaultConnection")));
			services.AddDefaultIdentity<IdentityUser>(config =>
			{
				config.Password.RequireNonAlphanumeric = false;
				config.Password.RequireUppercase = false;
				config.Password.RequireDigit = false;
			}).AddEntityFrameworkStores<ApplicationDbContext>();


			services.AddTransient<IEmailSender, EmailSender>();
			services.AddTransient<ProfileService>();
			services.Configure<AuthMessageSenderOptions>(Configuration);

			services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
			services.AddOptions();
			services.Configure<PayPalOptions>(Configuration.GetSection("PayPal"));
			
			var clientId = Configuration["Authentication:Google:ClientId"];

			services.AddAuthentication()
				.AddGoogle(googleOptions =>
				{
					googleOptions.ClientId = Configuration["Authentication:Google:ClientId"];
					googleOptions.ClientSecret = Configuration["Authentication:Google:ClientSecret"];
				});

			// In production, the React files will be served from this directory
			services.AddSpaStaticFiles(configuration =>
			{
				configuration.RootPath = "ClientApp/build";
			});
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
		{
			loggerFactory.AddSerilog();

			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseDatabaseErrorPage();
			}
			else
			{
				app.UseExceptionHandler("/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseStaticFiles();
			app.UseSpaStaticFiles();
			app.UseCookiePolicy();

			app.UseAuthentication();

			app.UseMvc(routes =>
			{
				routes.MapRoute(
					name: "default",
					template: "{controller}/{action=Index}/{id?}");
			});

			app.UseSpa(spa =>
			{
				spa.Options.SourcePath = "ClientApp";

				if (env.IsDevelopment())
				{
					spa.UseReactDevelopmentServer(npmScript: "start");
				}
			});
		}
	}
}
