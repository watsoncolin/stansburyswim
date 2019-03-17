using System;

namespace FillThePool.Models
{
    public class Transaction : IAuditInfo
    {
        public int TransactionId { get; set; }
		public DateTime TimeStamp { get; set; }
		public string PayPalToken { get; set; }
		public string PayPalPaymentId { get; set; }
		public string PayPalPayerId { get; set; }
        public string PayPalAccessToken { get; set; }
        public string Description { get; set; }
        public int LessonCredit { get; set; }
        public decimal Amount { get; set; }
        public string Type { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }

        virtual public PromoCode PromoCode { get; set; }
        virtual public User User { get; set; }

    }
}