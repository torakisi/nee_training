using System;

namespace NEE.Database
{
	public interface IProvideCreatedAndModified
	{
		string Id { get; set; }
		string EntityId { get; set; }
		DateTime CreatedAt { get; set; }
		string CreatedBy { get; set; }
		DateTime ModifiedAt { get; set; }
		string ModifiedBy { get; set; }
		int Revision { get; set; }
	}
}
