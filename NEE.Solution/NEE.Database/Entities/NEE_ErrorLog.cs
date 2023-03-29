using NEE.Core.Contracts.Enumerations;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NEE.Database.Entities
{
	public class NEE_ErrorLog
	{
		[Key, Column(Order = 0)]
		[Required]
		public string Id { get; set; }
		public string User { get; set; }
		public DateTime CreatedAt { get; set; }
		public string Exception { get; set; }
		public string ApplicationId { get; set; }
		public string StackTrace { get; set; }
		public string InnerException { get; set; }
		public ErrorLogSource ErrorLogSource { get; set; }


		public NEE_ErrorLog()
		{
			Id = Guid.NewGuid().ToString();
		}


	}
}
