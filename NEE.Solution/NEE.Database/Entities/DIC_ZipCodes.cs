using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NEE.Database.Entities
{
	public class DIC_ZipCodes
	{
		[Key, Column(Order = 0)]
		[Required]
		public long Id { get; set; }
		public string Code { get; set; }
		public string City { get; set; }
		public string District { get; set; }

	}
}
