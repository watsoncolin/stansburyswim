using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using FillThePool.Data;
using PayPal;
using PayPal.Api.Payments;
using WebMatrix.WebData;

namespace FillThePool.Web
{
    public static class PayPalTransaction
    {        
        public static string StartTransaction(string total, string description, string cancelUrl, string returnUrl, int credits)
        {
	        var unit = new ApplicationUnit();

	        var paypalAppId = Config.PayPalAppId;
			var paypalSecret = Config.PayPalAppSecret;
            //pass over to paypal
            var sdkConfig = new Dictionary<string, string> {{"mode", (Config.PayPalLive) ? "live" : "sandbox"}};
			System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
	        var accessToken = new OAuthTokenCredential(paypalAppId, paypalSecret, sdkConfig).GetAccessToken();
            var apiContext = new APIContext(accessToken) {Config = sdkConfig};
	        var amnt = new Amount {currency = "USD", total = total};
            var tran = new Transaction {description = description, amount = amnt};
	        var transactionList = new List<Transaction> {tran};
	        var payr = new Payer {payment_method = "paypal"};
	        var redirUrls = new RedirectUrls {cancel_url = cancelUrl, return_url = returnUrl};
	        var pymnt = new Payment
	        {
		        intent = "sale",
		        payer = payr,
		        transactions = transactionList,
		        redirect_urls = redirUrls
	        };

	        var createdPayment = pymnt.Create(apiContext);


            var approvalUrl = new Links();
            foreach (var l in createdPayment.links.Where(l => l.rel == "approval_url"))
	            approvalUrl = l;

	        if (approvalUrl.href == null) return "";
	       
	        var token = approvalUrl.href.Substring(approvalUrl.href.IndexOf("token=") + 6);
	        //save a new transaction and set it to pending capture payment id to reference later
	        unit.Transactions.Add(new FillThePool.Models.Transaction
	        {
		        Amount = 0,
		        LessonCredit = credits,
		        Description = "{PENDING}Purchased " + credits  + " Lessons",
		        PayPalToken = token,
		        PayPalAccessToken = accessToken,
		        PayPalPaymentId = createdPayment.id,
		        TimeStamp = DateTime.Now,
		        Type = "Purchased",
		        User = unit.Users.GetById(WebSecurity.CurrentUserId)
	        });
	        unit.SaveChanges();

	        return approvalUrl.href;
        }

        public static void CompleteTransaction(string token, string payerId)
        {
	        var unit = new ApplicationUnit();

	        var transaction = unit.Transactions.GetAll().First(x => x.PayPalToken == token && x.User.UserId == WebSecurity.CurrentUserId);
            var accessToken = transaction.PayPalAccessToken;

			var sdkConfig = new Dictionary<string, string> { { "mode", (Config.PayPalLive) ? "live" : "sandbox" } };
	        var apiContext = new APIContext(accessToken) {Config = sdkConfig};

	        var payment = new Payment {id = transaction.PayPalPaymentId};
	        var pymntExecution = new PaymentExecution {payer_id = transaction.PayPalPayerId = payerId};
	        var executedPayment = payment.Execute(apiContext, pymntExecution);

            if (executedPayment.state == "approved")
            {
                transaction.Amount = Decimal.Parse(executedPayment.transactions[0].amount.total);
                transaction.Description = transaction.Description.Replace("{PENDING}", "");

                var template = unit.EmailTemplates.GetAll().FirstOrDefault(x => x.Type == "Purchase");
	            var firstOrDefault = unit.Users.GetAll().FirstOrDefault(x => x.UserId == WebSecurity.CurrentUserId);
	            if (firstOrDefault != null)
		            if (template != null) 
						Email.SendEmail(firstOrDefault.Email, template.Subject, template.Html);
            }
            unit.SaveChanges();	
		}
    }
}