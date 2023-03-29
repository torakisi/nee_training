using NEE.Core.Contracts.Enumerations;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NEE.Database.Entities
{
	public class NEE_ChangeLog
	{
		[Key, Column(Order = 0)]
		[Required]
		public string Id { get; set; }

		[Key, Column(Order = 1)]
		[Required]
		public string ChangeLogId { get; set; }

		public string User { get; set; }
		public DateTime ModifiedAt { get; set; }
		public string ChangedField { get; set; }
		public string OriginalValue { get; set; }
		public string CurrentValue { get; set; }
		public string EntityType { get; set; }
		public int? Revision { get; set; }
		public string EntityId { get; set; }
		public ChangeLogTypes Type { get; set; }
	}
}
