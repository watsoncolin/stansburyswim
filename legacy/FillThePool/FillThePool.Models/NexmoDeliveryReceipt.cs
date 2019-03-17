
namespace FillThePool.Models
{
	public class NexmoDeliveryReceipt
	{
		public string To { get; set; }
		public string NetworkCode { get; set; }
		public string MessageId { get; set; }
		public string Msisdn { get; set; }
		public string Status { get; set; }
		public string ErrCode { get; set; }
		public string Price { get; set; }
		public string Scts { get; set; }
		public string MessageTimestamp { get; set; }
		public string ClientRef { get; set; }
	}
}
