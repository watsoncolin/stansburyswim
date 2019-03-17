using DotNetOpenAuth.AspNet;
using FillThePool.Data;
using FillThePool.Models;
using FillThePool.Web.ViewModels;
using Microsoft.Web.WebPages.OAuth;
using System;
using System.Linq;
using System.Transactions;
using System.Web.Mvc;
using System.Web.Security;
using WebMatrix.WebData;

namespace FillThePool.Web.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
	    private readonly ApplicationUnit _unit = new ApplicationUnit();

        //
        // GET: /Account/Login

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid && WebSecurity.Login(model.UserName, model.Password, model.RememberMe))
            {
                return RedirectToLocal(returnUrl);
            }

            // If we got this far, something failed, redisplay form
            ModelState.AddModelError("", "The user name or password provided is incorrect.");
            return View(model);
        }

        //
        // POST: /Account/LogOff

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            WebSecurity.Logout();

            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/Register

        [AllowAnonymous]
        public ActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        //
        // POST: /Account/Register

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel model)
        {
	        if (!ModelState.IsValid) return View(model);
	        // Attempt to register the user
	        try
	        {
		        WebSecurity.CreateUserAndAccount(model.UserName, model.Password, new {model.Email, model.Phone, CreatedOn = DateTime.Now, ModifiedOn = DateTime.Now, model.PrivateRegistration});
		        WebSecurity.Login(model.UserName, model.Password);

		        var user = _unit.Users.GetAll().FirstOrDefault(x => x.UserName == model.UserName);
		        if (user != null)
		        {
			        user.Email = model.Email;
			        user.Address = model.Address;
			        user.Phone = model.Phone;
			        user.FirstName = model.FirstName;
			        user.LastName = model.LastName;
			        user.Address2 = model.Address2;
			        user.City = model.City;
			        user.State = model.State;
			        user.Zip = model.Zip;
			        user.MobilePhone = model.MobilePhone;
			        user.MobileCarrier = model.MobileCarrier;
			        user.PrivateRegistration = model.PrivateRegistration;
			        user.Referral = model.Referral;
		        }
		        _unit.SaveChanges();
                    
		        var template = _unit.EmailTemplates.GetAll().FirstOrDefault(x => x.Type == "Registration");
		        if (template != null) Email.SendEmail(model.Email, template.Subject, template.Html);

		        return RedirectToAction("Index", "Home", new { Tour = true });
                    
	        }
	        catch (MembershipCreateUserException e)
	        {
		        ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
	        }

	        // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/Disassociate

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Disassociate(string provider, string providerUserId)
        {
            string ownerAccount = OAuthWebSecurity.GetUserName(provider, providerUserId);
            ManageMessageId? message = null;

            // Only disassociate the account if the currently logged in user is the owner
            if (ownerAccount == User.Identity.Name)
            {
                // Use a transaction to prevent the user from deleting their last login credential
                using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.Serializable }))
                {
                    bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
                    if (hasLocalAccount || OAuthWebSecurity.GetAccountsFromUserName(User.Identity.Name).Count > 1)
                    {
                        OAuthWebSecurity.DeleteAccount(provider, providerUserId);
                        scope.Complete();
                        message = ManageMessageId.RemoveLoginSuccess;
                    }
                }
            }

            return RedirectToAction("Manage", new { Message = message });
        }

        //
        // GET: /Account/Manage

        public ActionResult Manage(ManageMessageId? message)
        {
			var vm = new AccountViewModel
			{
				User = _unit.Users.GetById(WebSecurity.CurrentUserId),
				Students = _unit.Students.GetAll().Where(m => m.User.UserId == WebSecurity.CurrentUserId).ToList()
			};
	        ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : "";
            ViewBag.HasLocalPassword = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View(vm);
        }

        //
        // POST: /Account/Manage

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Manage(AccountViewModel model)
        {
            bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            ViewBag.HasLocalPassword = hasLocalAccount;
            ViewBag.ReturnUrl = Url.Action("Manage");
            if (hasLocalAccount)
            {
                if (ModelState.IsValid)
                {
                    // ChangePassword will throw an exception rather than return false in certain failure scenarios.
                    bool changePasswordSucceeded;
                    try
                    {
                        changePasswordSucceeded = WebSecurity.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword);
                    }
                    catch (Exception)
                    {
                        changePasswordSucceeded = false;
                    }

                    if (changePasswordSucceeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                    }
	                ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                }
            }
            else
            {
                // User does not have a local password so remove any validation errors caused by a missing
                // OldPassword field
                ModelState state = ModelState["OldPassword"];
                if (state != null)
                {
                    state.Errors.Clear();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        WebSecurity.CreateAccount(User.Identity.Name, model.NewPassword);
                        return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
                    }
                    catch (Exception)
                    {
                        ModelState.AddModelError("", String.Format("Unable to create local account. An account with the name \"{0}\" may already exist.", User.Identity.Name));
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/ExternalLogin

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            return new ExternalLoginResult(provider, Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/ExternalLoginCallback

        [AllowAnonymous]
        public ActionResult ExternalLoginCallback(string returnUrl)
        {
            AuthenticationResult result = OAuthWebSecurity.VerifyAuthentication(Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
            if (!result.IsSuccessful)
            {
                return RedirectToAction("ExternalLoginFailure");
            }

            if (OAuthWebSecurity.Login(result.Provider, result.ProviderUserId, createPersistentCookie: false))
            {
                return RedirectToLocal(returnUrl);
            }

            if (User.Identity.IsAuthenticated)
            {
                // If the current user is logged in add the new account
                OAuthWebSecurity.CreateOrUpdateAccount(result.Provider, result.ProviderUserId, User.Identity.Name);
                return RedirectToLocal(returnUrl);
            }
	        // User is new, ask for their desired membership name
	        var loginData = OAuthWebSecurity.SerializeProviderUserId(result.Provider, result.ProviderUserId);
	        ViewBag.ProviderDisplayName = OAuthWebSecurity.GetOAuthClientData(result.Provider).DisplayName;
	        ViewBag.ReturnUrl = returnUrl;
	        return View("ExternalLoginConfirmation", new RegisterExternalLoginModel { UserName = result.UserName, ExternalLoginData = loginData });
        }

        //
        // POST: /Account/ExternalLoginConfirmation

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLoginConfirmation(RegisterExternalLoginModel model, string returnUrl)
        {
            string provider;
            string providerUserId;

            if (User.Identity.IsAuthenticated || !OAuthWebSecurity.TryDeserializeProviderUserId(model.ExternalLoginData, out provider, out providerUserId))
            {
                return RedirectToAction("Manage");
            }

            if (ModelState.IsValid)
            {
                // Insert a new user into the database
                using (var unit = new ApplicationUnit())
                {
					User user = unit.Users.GetAll().FirstOrDefault(u => u.UserName.ToLower() == model.UserName.ToLower());
                    // Check if user already exists
                    if (user == null)
                    {
                        // Insert name into the profile table
                        unit.Users.Add(new User 
						{ 
							UserName = model.UserName, 
							FirstName = model.FirstName, 
							LastName = model.LastName, 
							Address = model.Address, 
							Email = model.Email, 
							City = model.City,
							State = model.State,
							Zip = model.Zip,
							MobilePhone = model.MobilePhone,
							MobileCarrier = model.MobileCarrier,
							Phone = model.Phone,
							PrivateRegistration = model.PrivateRegistration,
							Referral = model.Referral
						});
                        unit.SaveChanges();

                        OAuthWebSecurity.CreateOrUpdateAccount(provider, providerUserId, model.UserName);
                        OAuthWebSecurity.Login(provider, providerUserId, createPersistentCookie: false);

						EmailTemplate template = unit.EmailTemplates.GetAll().FirstOrDefault(x => x.Type == "Registration");
	                    if (template != null) Email.SendEmail(model.Email, template.Subject, template.Html);

	                    return RedirectToAction("Index", "Home", new { Tour = true });
                    }
	                ModelState.AddModelError("UserName", "User name already exists. Please enter a different user name.");
                }
            }

            ViewBag.ProviderDisplayName = OAuthWebSecurity.GetOAuthClientData(provider).DisplayName;
            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // GET: /Account/ExternalLoginFailure

        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        [AllowAnonymous]
        [ChildActionOnly]
        public ActionResult ExternalLoginsList(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return PartialView("_ExternalLoginsListPartial", OAuthWebSecurity.RegisteredClientData);
        }

        [ChildActionOnly]
        public ActionResult RemoveExternalLogins()
        {
            var accounts = OAuthWebSecurity.GetAccountsFromUserName(User.Identity.Name);
            var externalLogins = (from account in accounts
	            let clientData = OAuthWebSecurity.GetOAuthClientData(account.Provider)
	            select new ExternalLogin
	            {
		            Provider = account.Provider, ProviderDisplayName = clientData.DisplayName, ProviderUserId = account.ProviderUserId,
	            }).ToList();

	        ViewBag.ShowRemoveButton = externalLogins.Count > 1 || OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            return PartialView("_RemoveExternalLoginsPartial", externalLogins);
        }

		[AllowAnonymous]
		public ActionResult ForgotPassword()
		{
			
			return View();
		}

		[AllowAnonymous]
		[HttpPost]
		public ActionResult ForgotPassword(ForgotPasswordViewModel model)
		{
			var unit = new ApplicationUnit();

			var user = unit.Users.GetAll().FirstOrDefault(u => u.Email == model.Email);
			
			if (user != null)
			{
                ViewBag.Message = "Please check your email for more instructions.";
                try
                {
                    if (OAuthWebSecurity.GetAccountsFromUserName(user.UserName).Count > 0)
                    {
                        ViewBag.Message =
                            "Looks like you've used a third-party system to login try logging in with facebook or google.";
                    }
                    else
                    {
                        var token = WebSecurity.GeneratePasswordResetToken(user.UserName);
                        var message = string.Format("Here is your password reset link that you requested." +
                                                    "If you did not request to reset your password you can safely ignore this email.  " +
													"Please follow the following link to reset your password.<br> <a href='https://www.stansburyswim.com/Account/ResetPassword/?token={0}'>https://www.stansburyswim.com/Account/ResetPassword/?token={0}</a>",
                            token);

                        Email.SendEmail(user.Email, "Reset your Stansbury Swim Password", message);
                    }
                }
                catch
                {
                    ViewBag.Message = "Sorry there was a problem sending a password reset email.  Please check your email address and try again.";
                }
            }
            else
            {
                ViewBag.Message = "Sorry there was a problem sending a password reset email.  Please check your email address and try again.";
            }
            return View();
		}

        [AllowAnonymous]
        public ActionResult ResetPassword(string token)
        {
            if (token != null)
            {

                var unit = new ApplicationUnit();
                var userId = WebSecurity.GetUserIdFromPasswordResetToken(token);

                var user = unit.Users.GetById(userId);

                if (user != null)
                {
                    var model = new ResetPasswordViewModel();
                    model.Token = token;
                    return View(model);
                }
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult ResetPassword(ResetPasswordViewModel model)
        {
            var userId = WebSecurity.GetUserIdFromPasswordResetToken(model.Token);
            
            var unit = new ApplicationUnit();
            var user = unit.Users.GetById(userId);

            if (user != null)
            {
                var success = WebSecurity.ResetPassword(model.Token, model.NewPassword);

                if (success)
                {
                    if(WebSecurity.Login(user.UserName, model.NewPassword))
                       return RedirectToAction("Index", "Home");
                }
            }

            ViewBag.Message = "We're sorry but something went wrong :(  Please try again.";

            return View();
        }


		protected override void Dispose(bool disposing)
		{
			_unit.Dispose();
			base.Dispose(disposing);
		}

        #region Helpers
        private ActionResult RedirectToLocal(string returnUrl)
        {
	        if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
	        return RedirectToAction("Index", "Home");
        }

	    public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
        }

        internal class ExternalLoginResult : ActionResult
        {
            public ExternalLoginResult(string provider, string returnUrl)
            {
                Provider = provider;
                ReturnUrl = returnUrl;
            }

            public string Provider { get; private set; }
            public string ReturnUrl { get; private set; }

            public override void ExecuteResult(ControllerContext context)
            {
                OAuthWebSecurity.RequestAuthentication(Provider, ReturnUrl);
            }
        }

        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion
    }
}
