﻿
namespace FillThePool.Models
{
	public class NexmoInboundMessage
	{
		public string Type { get; set; }
		public string To { get; set; }
		public string Msisdn { get; set; }
		public string NetworkCode { get; set; }
		public string MessageId { get; set; }
		public string MessageTimestamp { get; set; }
		public string Text { get; set; }
		public string Concat { get; set; }
		public string ConcatRef { get; set; }
		public string ConcatTotal { get; set; }
		public string ConcatPart { get; set; }
		public string Data { get; set; }
		public string Udh { get; set; }
	}
}
