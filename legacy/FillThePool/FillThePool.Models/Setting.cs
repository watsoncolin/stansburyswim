using System;

namespace FillThePool.Models
{
	public class Setting : IAuditInfo
	{
		public int SettingId { get; set; }
		public string Key { get; set; }
        public string Value { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
	}
}