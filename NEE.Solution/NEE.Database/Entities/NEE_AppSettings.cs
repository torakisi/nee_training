using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NEE.Database
{
	public class NEE_AppSettings
	{
		[Key, Column(Order = 0)]
		[Required]
		public string SettingId { get; set; }
		public bool DisableDBUpdates { get; set; }
	}
}